using Microsoft.EntityFrameworkCore;
using CityApi.Data;
using CityApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.Logger.LogInformation("CityApi APPLICATION STARTED");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        if (!context.Cities.Any())
        {
            context.Cities.AddRange(
                new City { Name = "New York" },
                new City { Name = "London" },
                new City { Name = "Tokyo" },
                new City { Name = "Paris" },
                new City { Name = "Sydney" }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "City API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the root
    });
}

app.UseHttpLogging();

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("API Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

// app.UseHttpsRedirection();

app.MapGet("/api/cities", async (AppDbContext context) =>
{
    return await context.Cities.ToListAsync();
})
.WithName("GetCities");

app.Run();

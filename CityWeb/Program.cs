using CityWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

// Register HttpClient to talk to CityApi
builder.Services.AddHttpClient("CityApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:CityApiBaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

app.Logger.LogInformation("CityWeb APPLICATION STARTED");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpLogging();

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Page Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

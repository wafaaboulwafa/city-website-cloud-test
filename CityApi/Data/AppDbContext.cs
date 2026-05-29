using Microsoft.EntityFrameworkCore;
using CityApi.Models;

namespace CityApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; } = null!;
    }
}

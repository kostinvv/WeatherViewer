using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data.EntityTypeConfigurations;
using WeatherViewer.Models;

namespace WeatherViewer.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Location> Locations => Set<Location>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
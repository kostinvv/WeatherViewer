using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data.EntityTypeConfigurations;
using WeatherViewer.Models;

namespace WeatherViewer.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Location> Locations => Set<Location>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
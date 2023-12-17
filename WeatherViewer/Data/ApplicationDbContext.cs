using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data.EntityTypeConfigurations;
using WeatherViewer.Models;

namespace WeatherViewer.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User>? Users { get; set; }
    public DbSet<Session>? Sessions { get; set; }
    public DbSet<Location>? Locations { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
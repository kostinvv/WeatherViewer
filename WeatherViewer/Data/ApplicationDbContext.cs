using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WeatherViewer.Models;

namespace WeatherViewer.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Location> Locations => Set<Location>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)  { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}
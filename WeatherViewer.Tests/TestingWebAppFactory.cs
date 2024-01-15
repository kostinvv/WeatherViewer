using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherViewer.Data;
using WeatherViewer.Models;

namespace WeatherViewer.Tests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    private IConfiguration _configuration;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Integration.json"));
            
            _configuration = configuration.Build();
        });
        
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
            );
                
            // unregister dbContext
            if (descriptor != null)
                services.Remove(descriptor);
            
            // add database test context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
                
            var serviceProvider = services.BuildServiceProvider();
            
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // database reset
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Add(new User()
            {
                Login = "user_test_login", 
                Password = BCrypt.Net.BCrypt.HashPassword("TestPassword123"),
            });
            context.SaveChanges();
        });
    }
}
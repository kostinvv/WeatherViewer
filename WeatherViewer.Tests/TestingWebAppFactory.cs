namespace WeatherViewer.Tests;

public class TestingWebAppFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Utilities.ReinitializeDbForTests(context);
        });

        builder.UseEnvironment("Development");
    }
}
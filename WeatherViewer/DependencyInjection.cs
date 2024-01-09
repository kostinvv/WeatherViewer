using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.Services;
using WeatherViewer.Services.Interfaces;

namespace WeatherViewer;

public static class DependencyInjection
{
    /// <summary> Add services to IoC. </summary>
    /// <param name="services"> IoC </param>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        
        services.AddHttpClient();
        
        services.AddTransient<IAuthService, AuthService>();
        
        services.AddTransient<IWeatherService, WeatherService>();
        
        services.AddTransient<SessionService>();
        
        services.AddHostedService<SessionHostedService>();
    }

    /// <summary> Add database configuration. </summary>
    /// <param name="services"> IoC </param>
    /// <param name="configuration"> Configuration </param>
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(optionsAction: options 
            => options.UseNpgsql(
                connectionString: configuration.GetConnectionString("DefaultConnection")));
    }
}
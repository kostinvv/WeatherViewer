using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;

namespace WeatherViewer.Services;

public class SessionService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SessionService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task DeleteExpiredSessions()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database
            .ExecuteSqlRawAsync("DELETE FROM sessions WHERE expires_at < CURRENT_TIMESTAMP");
    }
}
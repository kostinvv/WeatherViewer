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
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
        var expiredSessions = await (dbContext.Sessions ?? throw new InvalidOperationException())
            .Where(session => session.ExpiresAt < DateTime.UtcNow).ToListAsync();
            
        foreach (var session in expiredSessions)
            dbContext.Sessions.Remove(session);
        
        await dbContext.SaveChangesAsync();
    }
}
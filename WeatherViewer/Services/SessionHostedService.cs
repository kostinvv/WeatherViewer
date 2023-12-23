namespace WeatherViewer.Services;

public class SessionHostedService : BackgroundService
{
    private readonly SessionService _sessionService;

    public SessionHostedService(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _sessionService.DeleteExpiredSessions();

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
namespace WeatherViewer.Services;

public class SessionHostedService : BackgroundService
{
    private readonly SessionService _sessionService;
    private readonly ILogger<SessionHostedService> _logger;

    public SessionHostedService(SessionService sessionService, ILogger<SessionHostedService> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Deleting expired sessions.");
            await _sessionService.DeleteExpiredSessions();

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
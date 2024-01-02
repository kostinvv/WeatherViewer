namespace WeatherViewer.Services;

public class SessionHostedService : BackgroundService
{
    private readonly SessionService _sessionService;
    private readonly ILogger<SessionHostedService> _logger;
    private readonly IConfiguration _config;

    public SessionHostedService(
        SessionService sessionService, 
        ILogger<SessionHostedService> logger, 
        IConfiguration config)
    {
        _sessionService = sessionService;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Deleting expired sessions.");
            await _sessionService.DeleteExpiredSessions();
            
            var minutes = double.Parse(_config["MaxAge"] ?? throw new InvalidOperationException());
            await Task.Delay(TimeSpan.FromMinutes(minutes), stoppingToken);
        }
    }
}
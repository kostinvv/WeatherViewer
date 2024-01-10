namespace WeatherViewer.Models;

public record Session
{
    public Guid SessionId { get; init; }
    public long UserId { get; init; }
    public TimeSpan AbsoluteExpireTime { get; init; }
}
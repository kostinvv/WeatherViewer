namespace WeatherViewer.Models.DTOs.Auth;

public record LoginRequestDto
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}
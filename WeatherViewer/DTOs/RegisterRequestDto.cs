namespace WeatherViewer.DTOs;

public record RegisterRequestDto
{
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Email { get; init; }
}
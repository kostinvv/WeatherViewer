namespace WeatherViewer.Models.DTOs;

public record ForecastDto
{
    public required string Icon { get; init; }
    public required string Month { get; init; }
    public required string Temp { get; init; }
    public required string Description { get; init; }
}
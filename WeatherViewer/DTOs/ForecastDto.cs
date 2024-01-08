namespace WeatherViewer.DTOs;

public record ForecastDto
{
    public string Icon { get; init; }
    public string Month { get; init; }
    public string Temp { get; init; }
    public string Description { get; init; }
}
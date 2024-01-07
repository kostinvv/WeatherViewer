namespace WeatherViewer.DTOs;

public record LocationRequestDto
{
    public required string Name { get; init; }
    public required double Lat { get; init; }
    public required double Lon { get; init; }
}
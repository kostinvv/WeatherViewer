namespace WeatherViewer.Models.DTOs;

public class LocationDto
{
    public required string Name { get; init; }
    public required double Lat { get; init; }
    public required double Lon { get; init; }
}
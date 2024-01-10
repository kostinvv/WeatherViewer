namespace WeatherViewer.Models.DTOs;

public record WeatherDto
{
    /// <summary> Location id. </summary>
    public required long Id { get; init; }
    
    /// <summary> City name. </summary>
    public required string Name { get; init; }
    
    /// <summary> Weather station name. </summary>
    public required string WeatherStation { get; init; }
    
    /// <summary> Weather condition. </summary>
    public required string Description { get; init; }
    
    /// <summary> Visibility, meter. </summary>
    public required double Visibility { get; init; }
    
    /// <summary> Weather icon id. </summary>
    public required string Icon { get; init; }
    
    /// <summary> Country code. </summary>
    public required string Country { get; init; }
    
    /// <summary> Temperature. </summary>
    public required string Temp { get; init; }
    
    /// <summary> Temperature. Human perception of weather. </summary>
    public required string FeelsLike { get; init; }
    
    /// <summary> Maximum temperature at the moment. </summary>
    public required string TempMax { get; init; }
    
    /// <summary> Minimum temperature at the moment. </summary>
    public required string TempMin { get; init; }
    
    /// <summary> Wind speed (meter/sec). </summary>
    public required string Speed { get; init; }
    
    /// <summary> Humidity, % </summary>
    public required string Humidity { get; init; }
}
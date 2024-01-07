using Newtonsoft.Json;

namespace WeatherViewer.DTOs;

public record FoundLocationDto
{
    [JsonProperty(PropertyName = "name")]
    public required string Name { get; init; }
    
    [JsonProperty(PropertyName = "lat")]
    public required double Latitude { get; init; }
    
    [JsonProperty(PropertyName = "lon")]
    public required double Longitude { get; init; }
    
    [JsonProperty(PropertyName = "state")]
    public required string State { get; init; }
    
    [JsonProperty(PropertyName = "country")]
    public required string Country { get; init; }
}
using Newtonsoft.Json;
using WeatherViewer.Models.API.Common;

namespace WeatherViewer.Models.API;

public record ApiWeatherResponse
{
    [JsonProperty(PropertyName = "name")]
    public required string Name { get; init; }
    
    [JsonProperty(PropertyName = "visibility")]
    public required double Visibility { get; init; }
    
    [JsonProperty(PropertyName = "wind")]
    public required Wind Wind { get; init; }
    
    [JsonProperty(PropertyName = "coord")]
    public required Coordinates Coordinates { get; init; }
    
    [JsonProperty(PropertyName = "weather")]
    public required List<Weather> Weather { get; init; }
    
    [JsonProperty(PropertyName = "main")]
    public required Main Main { get; init; }
    
    [JsonProperty(PropertyName = "sys")]
    public required Sys Sys { get; init; }
}
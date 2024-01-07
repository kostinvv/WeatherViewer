using Newtonsoft.Json;

namespace WeatherViewer.DTOs;

public record WeatherDto
{
    [JsonProperty(PropertyName = "name")]
    public required string Name { get; init; }
    
    [JsonProperty(PropertyName = "coord")]
    public required Coordinates Coordinates { get; init; }
    
    [JsonProperty(PropertyName = "weather")]
    public required List<Weather> Weather { get; init; }
    
    [JsonProperty(PropertyName = "main")]
    public required Main Main { get; init; }
}

public class Weather
{
    [JsonProperty(PropertyName = "main")]
    public string Main { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; } = string.Empty;
}

public class Main
{
    [JsonProperty(PropertyName = "temp")]
    public string Temp { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "feels_like")]
    public string FeelsLike { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "temp_min")]
    public string TempMin { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "temp_max")]
    public string TempMax { get; set; } = string.Empty;
}

public class Coordinates
{
    [JsonProperty(PropertyName = "lon")]
    public string Lon { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "lat")]
    public string Lat { get; set; } = string.Empty;
}
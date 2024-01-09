using Newtonsoft.Json;

namespace WeatherViewer.Models.API.Common;

public class Main
{
    [JsonProperty(PropertyName = "temp")]
    public string Temp { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "feels_like")]
    public string FeelsLike { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "humidity")]
    public string Humidity { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "temp_min")]
    public string TempMin { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "temp_max")]
    public string TempMax { get; set; } = string.Empty;
}
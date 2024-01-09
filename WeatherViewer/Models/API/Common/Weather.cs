using Newtonsoft.Json;

namespace WeatherViewer.Models.API.Common;

public class Weather
{
    [JsonProperty(PropertyName = "main")]
    public string Main { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "icon")]
    public string Icon { get; set; } = string.Empty;
}
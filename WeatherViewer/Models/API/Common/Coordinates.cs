using Newtonsoft.Json;

namespace WeatherViewer.Models.API.Common;

public class Coordinates
{
    [JsonProperty(PropertyName = "lon")]
    public string Lon { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "lat")]
    public string Lat { get; set; } = string.Empty;
}
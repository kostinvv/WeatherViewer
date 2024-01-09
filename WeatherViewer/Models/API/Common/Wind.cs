using Newtonsoft.Json;

namespace WeatherViewer.Models.API.Common;

public class Wind
{
    [JsonProperty(PropertyName = "speed")]
    public string Speed { get; set; } = string.Empty;
}
using Newtonsoft.Json;

namespace WeatherViewer.Models.API.Common;

public class Sys
{
    [JsonProperty(PropertyName = "country")]
    public string Country { get; set; } = string.Empty;
}
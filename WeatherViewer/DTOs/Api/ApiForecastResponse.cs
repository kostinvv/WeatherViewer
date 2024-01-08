using Newtonsoft.Json;

namespace WeatherViewer.DTOs.Api;

public record ApiForecastResponse
{
    [JsonProperty(PropertyName = "list")]
    public List<WeatherList> List { get; init; }
}

public class WeatherList
{
    [JsonProperty(PropertyName = "main")]
    public Main Main { get; set; }

    [JsonProperty(PropertyName = "weather")]
    public List<Weather> Weather { get; set; }
    
    [JsonProperty(PropertyName = "dt_txt")]
    public DateTime DtTxt { get; set; }
}
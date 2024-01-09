using Newtonsoft.Json;

namespace WeatherViewer.Models.API.Common;

public class Forecast
{
    [JsonProperty(PropertyName = "main")]
    public Main Main { get; set; }

    [JsonProperty(PropertyName = "weather")]
    public List<Weather> Weather { get; set; }
    
    [JsonProperty(PropertyName = "dt_txt")]
    public DateTime DtTxt { get; set; }
}
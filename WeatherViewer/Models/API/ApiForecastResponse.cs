using Newtonsoft.Json;
using WeatherViewer.Models.API.Common;

namespace WeatherViewer.Models.API;

public record ApiForecastResponse
{
    [JsonProperty(PropertyName = "list")]
    public required List<Forecast> Forecasts { get; init; }
}
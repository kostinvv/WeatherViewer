namespace WeatherViewer.Exceptions;

public class OpenWeatherApiException : Exception
{
    public OpenWeatherApiException() : base("No response.") { }
}
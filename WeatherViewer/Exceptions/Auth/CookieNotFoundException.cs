namespace WeatherViewer.Exceptions.Auth;

public class CookieNotFoundException : Exception
{
    public CookieNotFoundException() : base("Cookie not found.") { }
}
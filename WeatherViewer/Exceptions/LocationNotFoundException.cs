namespace WeatherViewer.Exceptions;

public class LocationNotFoundException : Exception
{
    public LocationNotFoundException() : base("Location not found.") { }
}
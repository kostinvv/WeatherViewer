namespace WeatherViewer.Exceptions;

public class SessionNotFoundException : Exception
{
    public SessionNotFoundException() : base("Session not found.") { }
    public SessionNotFoundException(string message) : base(message) { }
}
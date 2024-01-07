namespace WeatherViewer.Exceptions.Auth;

public class SessionNotFoundException : Exception
{
    public SessionNotFoundException() : base("Session not found.") { }
    public SessionNotFoundException(string message) : base(message) { }
}
namespace WeatherViewer.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("The specified user doesn't exist.") { }
    public UserNotFoundException(string message) : base(message) { }
}
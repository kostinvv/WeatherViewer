namespace WeatherViewer.Exceptions.Auth;

public class UserExistsException : Exception
{
    public UserExistsException() : base("A user with this login already exists.") { }
    public UserExistsException(string message) : base(message) { }
}
namespace WeatherViewer.Exceptions.Auth;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException() : base("Invalid password.") { }
    public InvalidPasswordException(string message) : base(message) { }
}
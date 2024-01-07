namespace WeatherViewer.Exceptions;

public class DeleteLocationException : Exception
{
    public DeleteLocationException() : base("Bad request.") { }

}
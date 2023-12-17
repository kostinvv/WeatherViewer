namespace WeatherViewer.Models;

public class User
{
    public long UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public ICollection<Location> Locations { get; } = new List<Location>();
    public ICollection<Session> Sessions { get; } = new List<Session>();
}
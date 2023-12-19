namespace WeatherViewer.Models;

public class Session
{
    public Guid SessionId { get; set; }
    
    // foreign key property 
    public long UserId { get; set; }
    // navigation property
    public User User = null!;
    
    public DateTime ExpiresAt { get; set; }
}
namespace WeatherViewer.Models;

public class Location
{
    public long LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // foreign key property 
    public long UserId { get; set; }
    // navigation property
    public User User = null!;
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
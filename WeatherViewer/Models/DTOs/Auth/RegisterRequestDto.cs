using System.ComponentModel.DataAnnotations;

namespace WeatherViewer.Models.DTOs.Auth;

public record RegisterRequestDto
{
    public required string Login { get; init; }
    
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,15}$", 
        ErrorMessage = "Password is too easy.")]
    public required string Password { get; init; }
    
    public required string Email { get; init; }
}
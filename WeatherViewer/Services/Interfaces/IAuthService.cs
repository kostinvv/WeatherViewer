using WeatherViewer.Models;
using WeatherViewer.Models.DTOs.Auth;

namespace WeatherViewer.Services.Interfaces;

public interface IAuthService
{
    public Task CreateUserAsync(RegisterRequestDto request);
    
    public Task<Session> CreateSessionAsync(LoginRequestDto request);
}
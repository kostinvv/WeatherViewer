using WeatherViewer.DTOs.Auth;
using WeatherViewer.Models;

namespace WeatherViewer.Services;

public interface IAuthService
{
    public Task CreateUserAsync(RegisterRequestDto request);
    public Task<Session> AuthAsync(LoginRequestDto request);
    public Task DeleteSessionAsync(string sessionId);
}
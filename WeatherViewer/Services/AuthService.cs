using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.Exceptions.Auth;
using WeatherViewer.Models;
using WeatherViewer.Models.DTOs.Auth;
using WeatherViewer.Services.Interfaces;

namespace WeatherViewer.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _config;

    public AuthService(
        ApplicationDbContext context, 
        ILogger<AuthService> logger, 
        IConfiguration config)
    {
        _context = context;
        _logger = logger;
        _config = config;
    }

    public async Task CreateUserAsync(RegisterRequestDto request)
    {
        var foundUser = await _context.Users
            .FirstOrDefaultAsync(user => user.Login == request.Login);
        
        if (foundUser is not null) throw new UserExistsException();
        
        _logger.LogInformation("Creating user.");
        await _context.Users.AddAsync(new User
        {
            Login = request.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        });
        await _context.SaveChangesAsync();
        _logger.LogInformation("User successfully created.");
    }

    public async Task<Session> CreateSessionAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(user => user.Login == request.Login);
        
        if (user is null) throw new UserNotFoundException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new InvalidPasswordException();

        var minutes = double.Parse(
            _config["MaxAge"] ?? throw new InvalidOperationException());
        
        _logger.LogInformation("Creating session.");
        Session session = new ()
        {
            SessionId = Guid.NewGuid(),
            UserId = user.UserId,
            AbsoluteExpireTime = TimeSpan.FromMinutes(minutes),
        };

        return session;
    }
}
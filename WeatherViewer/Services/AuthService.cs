using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.DTOs;
using WeatherViewer.Exceptions;
using WeatherViewer.Models;

namespace WeatherViewer.Services;

public class AuthService
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
        var foundUser = await (_context.Users ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(user => user.Login == request.Login);
        
        if (foundUser is not null) throw new UserExistsException();
        
        _logger.LogInformation("Creating user.");
        await _context.Users.AddAsync(new ()
        {
            Login = request.Login,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        });
        await _context.SaveChangesAsync();
        _logger.LogInformation("User successfully created.");
    }

    public async Task<Session> AuthAsync(LoginRequestDto request)
    {
        var user = await (_context.Users ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(user => user.Login == request.Login);
        
        if (user is null) throw new UserNotFoundException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new InvalidPasswordException();

        var minutes = int.Parse(_config["MaxAge"] ?? throw new InvalidOperationException());
        
        _logger.LogInformation("Creating session.");
        Session session = new ()
        {
            SessionId = new Guid(),
            UserId = user.UserId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(minutes),
        };
        
        await (_context.Sessions ?? throw new InvalidOperationException()).AddAsync(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        var session = await (_context.Sessions ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(x => x.SessionId == new Guid(sessionId));

        if (session is null) throw new SessionNotFoundException();
        
        _logger.LogInformation("Session deletion.");
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.DTOs;
using WeatherViewer.Models;

namespace WeatherViewer.Controllers;

public class UserController : Controller
{
    private const string CookieKey = "SessionId";
    
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequestDto request)
    {
        if (!ModelState.IsValid) return View();
        
        var foundUser = await (_context.Users ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(user => user.Login == request.Login);

        if (foundUser is not null) return View();

        var user = new User()
        {
            Login = request.Login,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Login", "User");
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequestDto request)
    {
        if (!ModelState.IsValid) return View();
        
        var user = await (_context.Users ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(user => user.Login == request.Login);

        if (user is null) return View();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) return View();
        
        await CreateSessionAsync(user);
        await _context.SaveChangesAsync();

        return Redirect("/");
    }

    public async Task<IActionResult> LogoutAsync()
    {
        var sessionId = Request.Cookies
            .FirstOrDefault(cookie => cookie.Key == CookieKey).Value;
        
        if (sessionId is null) return RedirectToAction("Login", "User");
        Response.Cookies.Delete(CookieKey);
        
        var session = (_context.Sessions ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(x => x.SessionId == new Guid(sessionId)).Result;

        if (session is null) return RedirectToAction("Login", "User");
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login", "User");
    }

    private async Task CreateSessionAsync(User user)
    {
        var session = new Session()
        {
            SessionId = new Guid(),
            UserId = user.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
        };

        await (_context.Sessions ?? throw new InvalidOperationException()).AddAsync(session);
        
        Response.Cookies.Append(CookieKey, session.SessionId.ToString());
    }
}
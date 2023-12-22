using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.Models;

namespace WeatherViewer.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
        {
            return RedirectToAction("login", "user");
        }

        var user = await GetCurrentUser();
        
        return View(user);
    }

    private async Task<User> GetCurrentUser()
    {
        var sessionId = Request.Cookies
            .FirstOrDefault(cookie => cookie.Key == "SessionId").Value;

        var session = await (_context.Sessions ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(x => x.SessionId == new Guid(sessionId));

        var user = await (_context.Users ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(x => session != null && x.UserId == session.UserId);

        return user ?? throw new NullReferenceException();
    }
}
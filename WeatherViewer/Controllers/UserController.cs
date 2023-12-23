using Microsoft.AspNetCore.Mvc;
using WeatherViewer.DTOs;
using WeatherViewer.Exceptions;
using WeatherViewer.Services;

namespace WeatherViewer.Controllers;

public class UserController : Controller
{
    private const string CookieKey = "SessionId";
    
    private readonly AuthService _authService;

    public UserController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequestDto request)
    {
        if (!ModelState.IsValid) return View();

        try
        {
            await _authService.CreateUserAsync(request);
            
            return RedirectToAction("login", "user");
        }
        catch (UserExistsException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var session = await _authService.AuthAsync(request);
            Response.Cookies.Append(CookieKey, session.SessionId.ToString());
            return Redirect("/");
        }
        catch (UserNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
        catch (InvalidPasswordException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            var sessionId = Request.Cookies
                .FirstOrDefault(cookie => cookie.Key == CookieKey).Value;

            if (sessionId is null) return RedirectToAction("login", "user");
            
            Response.Cookies.Delete(CookieKey);
            await _authService.DeleteSessionAsync(sessionId);
            
            return RedirectToAction("login", "user");
        }
        catch (SessionNotFoundException)
        {
            return RedirectToAction("login", "user");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using WeatherViewer.Exceptions.Auth;
using WeatherViewer.Models.DTOs.Auth;
using WeatherViewer.Services.Interfaces;
using WeatherViewer.Extensions;

namespace WeatherViewer.Controllers;

public class UserController : Controller
{
    private const string CookieSessionId = "SessionId";
    private const string CookieWeatherLogin = "weather_login";
    
    private readonly IAuthService _authService;
    private readonly IDistributedCache _cache;

    public UserController(
        IAuthService authService, 
        IDistributedCache cache)
    {
        _authService = authService;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromForm]RegisterRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid) return View();
            
            await _authService.CreateUserAsync(request);
            return RedirectToAction("login");
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
            if (!ModelState.IsValid) return View();
            
            var session = await _authService.CreateSessionAsync(request);
            var sessionId = session.SessionId.ToString();
            
            await _cache.SetRecordAsync(
                key: sessionId, 
                data: session.UserId, 
                absoluteExpireTime: session.AbsoluteExpireTime,
                unusedExpireTime: TimeSpan.FromDays(3));
            
            Response.Cookies.Append(CookieSessionId, value: sessionId, 
                new CookieOptions { MaxAge = session.AbsoluteExpireTime, });
            Response.Cookies.Append(CookieWeatherLogin, request.Login, 
                new CookieOptions { MaxAge = session.AbsoluteExpireTime, });
            
            return Redirect("/");
        }
        catch (UserNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (InvalidPasswordException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        if (Request.Cookies.ContainsKey(CookieSessionId))
            Response.Cookies.Delete(key:CookieSessionId);
        
        if (Request.Cookies.ContainsKey(CookieWeatherLogin))
            Response.Cookies.Delete(key:CookieWeatherLogin);

        return RedirectToAction("login", "user");
    }
}
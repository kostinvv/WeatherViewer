﻿using Microsoft.AspNetCore.Mvc;
using WeatherViewer.DTOs.Auth;
using WeatherViewer.Exceptions.Auth;
using WeatherViewer.Services;

namespace WeatherViewer.Controllers;

public class UserController : Controller
{
    private const string CookieKey = "SessionId";
    
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public UserController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
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
            return RedirectToAction("Login");
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
            
            var login = request.Login;
            var session = await _authService.AuthAsync(request);
            var minutes = double.Parse(_config["MaxAge"] ?? throw new InvalidOperationException());
            Response.Cookies.Append(CookieKey, session.SessionId.ToString(), new CookieOptions()
            {
                MaxAge = TimeSpan.FromMinutes(minutes),
            });
            Response.Cookies.Append("weather_login", login);
            
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
            
            Response.Cookies.Delete("weather_login");
            
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
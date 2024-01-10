using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using WeatherViewer.Models.DTOs;
using WeatherViewer.Services.Interfaces;
using WeatherViewer.Extensions;

namespace WeatherViewer.Controllers;

public class HomeController : Controller
{
    private readonly IWeatherService _service;
    private readonly IDistributedCache _cache;

    public HomeController(
        IWeatherService service, 
        IDistributedCache cache)
    {
        _service = service;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserIdAsync();
        var weather = await _service.GetWeatherAsync(userId);
        
        return View(weather);
    }
    
    [HttpPost("locations")]
    public async Task<IActionResult> GetLocationsAsync([FromForm] string name)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        if (!ModelState.IsValid)
            return RedirectToAction("index", "home");
        
        ViewData["LocationName"] = name;
        
        var foundLocations = await _service.SearchLocationsAsync(name);
        return View(foundLocations);
    }

    [HttpGet("delete/{locationId:long}")]
    public async Task<IActionResult> DeleteLocationAsync(long locationId)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserIdAsync();
        await _service.DeleteLocationAsync(locationId, userId);
        
        return RedirectToAction("index", "home");
    }
    
    [HttpGet("delete_all")]
    public async Task<IActionResult> DeleteAllLocationAsync()
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserIdAsync();
        await _service.DeleteAllLocationsAsync(userId);
        
        return RedirectToAction("index", "home");
    }

    [HttpPost]
    public async Task<IActionResult> AddLocationAsync([FromForm] LocationDto request)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserIdAsync();
        await _service.AddLocationAsync(request, userId);
        
        return RedirectToAction("index", "home");
    }

    [HttpGet("forecast/{id:long}")]
    public async Task<IActionResult> GetForecastAsync(long id)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserIdAsync();
        var forecast = await _service.GetWeatherForecastsAsync(locationId:id, userId);
        
        return View(forecast);
    }

    public IActionResult LocationForm()
    {
        return PartialView();
    }

    public IActionResult Error()
    {
        return View();
    }

    private async Task<long> GetUserIdAsync()
    {
        var sessionId = Request.Cookies.First(cookie =>
                cookie.Key == "SessionId").Value;
        
        return await _cache.GetRecordAsync<long>(key: sessionId);
    }
}
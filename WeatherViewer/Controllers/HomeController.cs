using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using WeatherViewer.Exceptions.Auth;
using WeatherViewer.Extensions;
using WeatherViewer.Filters;
using WeatherViewer.Models.DTOs;
using WeatherViewer.Services.Interfaces;
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

    public IActionResult LocationForm() => PartialView();
    
    public IActionResult Error() => View();
    
    [HttpGet]
    [SessionException]
    public async Task<IActionResult> IndexAsync()
    {
        var userId = await GetUserIdAsync();
        var weather = await _service.GetWeatherAsync(userId);
        return View(weather);
    }
    
    [HttpPost("locations")]
    [SessionException]
    public async Task<IActionResult> GetLocationsAsync([FromForm] string name)
    {
        await GetUserIdAsync();
        if (!ModelState.IsValid)
            return RedirectToAction("index", "home");
        
        ViewData["LocationName"] = name;
        
        var foundLocations = await _service.SearchLocationsAsync(name);
        return View(foundLocations);
    }
    
    [HttpGet("delete/{locationId:long}")]
    [SessionException]
    public async Task<IActionResult> DeleteLocationAsync(long locationId)
    {
        var userId = await GetUserIdAsync();
        await _service.DeleteLocationAsync(locationId, userId);
        
        return RedirectToAction("index", "home");
    }
    
    [HttpGet("delete_all")]
    [SessionException]
    public async Task<IActionResult> DeleteAllLocationAsync()
    {
        var userId = await GetUserIdAsync();
        await _service.DeleteAllLocationsAsync(userId);
        
        return RedirectToAction("index", "home");
    }
    
    [HttpPost]
    [SessionException]
    public async Task<IActionResult> AddLocationAsync([FromForm] LocationDto request)
    {
        var userId = await GetUserIdAsync();
        await _service.AddLocationAsync(request, userId);
        
        return RedirectToAction("index", "home");
    }
    
    [HttpGet("forecast/{id:long}")]
    [SessionException]
    public async Task<IActionResult> GetForecastAsync(long id)
    {
        var userId = await GetUserIdAsync();
        var forecast = await _service.GetWeatherForecastsAsync(locationId:id, userId);
        return View(forecast);
    }
    
    private async Task<long> GetUserIdAsync()
    {
        try
        {
            var cookie = Request.Cookies.First(cookie => 
                        cookie.Key == "SessionId");
            
            var userId = await _cache.GetRecordAsync<long>(key: cookie.Value);

            if (userId == default)
            {
                Response.Cookies.Delete(cookie.Key);
                throw new SessionNotFoundException();
            }

            return userId;
        }
        catch (InvalidOperationException)
        {
            throw new CookieNotFoundException();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using WeatherViewer.Models.DTOs;
using WeatherViewer.Services.Interfaces;
using WeatherViewer.Extensions;
using WeatherViewer.Filters;

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
    
    [Auth]
    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var userId = await GetUserIdAsync();
        var weather = await _service.GetWeatherAsync(userId);
        
        return View(weather);
    }
    
    [Auth]
    [HttpPost("locations")]
    public async Task<IActionResult> GetLocationsAsync([FromForm] string name)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("index", "home");
        
        ViewData["LocationName"] = name;
        
        var foundLocations = await _service.SearchLocationsAsync(name);
        return View(foundLocations);
    }

    [Auth]
    [HttpGet("delete/{locationId:long}")]
    public async Task<IActionResult> DeleteLocationAsync(long locationId)
    {
        try
        {
            var userId = await GetUserIdAsync();
            await _service.DeleteLocationAsync(locationId, userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return View("index");
    }
    
    [Auth]
    [HttpGet("delete_all")]
    public async Task<IActionResult> DeleteAllLocationAsync()
    {
        var userId = await GetUserIdAsync();
        await _service.DeleteAllLocationsAsync(userId);
        
        return RedirectToAction("index", "home");
    }

    [Auth]
    [HttpPost]
    public async Task<IActionResult> AddLocationAsync([FromForm] LocationDto request)
    {
        var userId = await GetUserIdAsync();
        await _service.AddLocationAsync(request, userId);
        
        return RedirectToAction("index", "home");
    }

    [Auth]
    [HttpGet("forecast/{id:long}")]
    public async Task<IActionResult> GetForecastAsync(long id)
    {
        var userId = await GetUserIdAsync();
        var forecast = await _service.GetWeatherForecastsAsync(locationId:id, userId);
        
        return View(forecast);
    }

    private async Task<long> GetUserIdAsync()
    {
        var sessionId = Request.Cookies.First(cookie =>
                cookie.Key == "SessionId").Value;
        
        return await _cache.GetRecordAsync<long>(key: sessionId);
    }
}
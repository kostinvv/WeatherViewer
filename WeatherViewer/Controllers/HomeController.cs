using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.DTOs;
using WeatherViewer.Exceptions;
using WeatherViewer.Services;

namespace WeatherViewer.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWeatherService _service;

    public HomeController(ApplicationDbContext context, IWeatherService service)
    {
        _context = context;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        return View(await CreateWeatherDictionary());
    }

    [HttpGet("delete")]
    public async Task<IActionResult> DeleteLocation(long locationId)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        try
        {
            var userId = await GetUserId();
            await _service.DeleteLocationAsync(locationId, userId);
        }
        catch (DeleteLocationException ex)
        {
        }
        
        return RedirectToAction("index", "home");
    }
    
    [HttpGet("delete_all")]
    public async Task<IActionResult> DeleteAllLocation()
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserId();
        await _service.DeleteAllLocationsAsync(userId);
        
        return RedirectToAction("index", "home");
    }
    
    [HttpPost("locations")]
    public async Task<IActionResult> GetLocationsAsync([FromForm] string name)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");

        if (name is "") RedirectToAction("index", "home");

        ViewData["LocationName"] = name;
        var foundLocations = await _service.SearchLocationsAsync(name);

        return View(foundLocations);
    }

    [HttpPost]
    public async Task<IActionResult> AddLocationAsync([FromForm] LocationRequestDto request)
    {
        if (!Request.Cookies.ContainsKey("SessionId"))
            return RedirectToAction("login", "user");
        
        var userId = await GetUserId();
        await _service.AddLocationAsync(request, userId);
        
        return RedirectToAction("index", "home");
    }

    public IActionResult LocationForm()
    {
        return PartialView();
    }

    private async Task<long> GetUserId()
    {
        // get session id from cookie
        var sessionId = Guid.Parse(
            Request.Cookies.First(cookie => 
                cookie.Key == "SessionId")
                .Value
            );
        
        // get user id from database
        var userId = await (_context.Sessions ?? throw new InvalidOperationException())
            .Where(s => s.SessionId == sessionId)
            .Select(s => s.UserId)
            .FirstAsync();

        return userId;
    }
    
    private async Task<Dictionary<long, WeatherDto>> CreateWeatherDictionary()
    {
        var weather = new Dictionary<long, WeatherDto>();
        
        var userId = await GetUserId();
        // get user locations
        var locations = await _service.GetLocationsAsync(userId);

        foreach (var location in locations)
        {
            var response = await _service.GetWeatherForLocationAsync(location);
            weather.Add(location.LocationId, response);
        }
        return weather;
    }
}
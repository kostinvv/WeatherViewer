using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.DTOs;
using WeatherViewer.DTOs.Api;
using WeatherViewer.Exceptions;
using WeatherViewer.Exceptions.Auth;
using WeatherViewer.Models;

namespace WeatherViewer.Services;

public class WeatherService : IWeatherService
{
    private const string Host = "api.openweathermap.org";
    private const int Limit = 5;
    
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeatherService> _logger;
    private readonly IConfiguration _config;

    public WeatherService(
        IHttpClientFactory httpClientFactory, 
        ApplicationDbContext context, 
        ILogger<WeatherService> logger, 
        IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _logger = logger;
        _config = config;
    }
    
    public async Task<IEnumerable<FoundLocationDto>> SearchLocationsAsync(string name)
    {
        try
        {
            var apiKey = _config["Weather:ServiceApiKey"];
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Path = "geo/1.0/direct",
                Query = $"q={name}&appid={apiKey}&limit={Limit}",
            };
        
            var requestUri = uriBuilder.Uri;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            
            var foundLocations = Newtonsoft.Json.JsonConvert
                .DeserializeObject<IEnumerable<FoundLocationDto>>(jsonString);
            
            return foundLocations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new OpenWeatherApiException();
        }
    }

    public async Task AddLocationAsync(LocationRequestDto requestDto, long userId)
    {
        Location location = new()
        {
            Name = requestDto.Name,
            UserId = userId,
            Latitude = requestDto.Lat,
            Longitude = requestDto.Lon,
        };

        await (_context.Locations ?? throw new InvalidOperationException())
            .AddAsync(location);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Location>> GetLocationsAsync(long userId) 
        => await (_context.Locations ?? throw new InvalidOperationException())
            .Where(location => location.UserId == userId)
            .ToListAsync();

    public async Task<WeatherDto> GetWeatherForLocationAsync(Location location)
    {
        try
        {
            var apiKey = _config["Weather:ServiceApiKey"];
            var lat = location.Latitude;
            var lon = location.Longitude;
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Path = "data/2.5/weather",
                Query = $"lat={lat}&lon={lon}&appid={apiKey}&units=metric",
            };

            var requestUri = uriBuilder.Uri;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var weather = Newtonsoft.Json.JsonConvert
                .DeserializeObject<WeatherDto>(jsonString);
            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new OpenWeatherApiException();
        }
        
    }

    public async Task DeleteLocationAsync(long locationId, long userId)
    {
        var location = await (_context.Locations ?? throw new InvalidOperationException())
            .FindAsync(locationId);

        if (location is null || location.UserId != userId)
            throw new DeleteLocationException();
        
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllLocationsAsync(long userId)
    {
        var locations = await (_context.Locations ?? throw new InvalidOperationException())
            .Where(location => location.UserId == userId)
            .ToListAsync();
        _context.Locations.RemoveRange(locations);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ForecastDto>> GetWeatherForecasts(long locationId, long userId)
    {
        var response = await GetWeatherForecastAsync(locationId, userId);
        var forecast = response.List.Select(value => new ForecastDto
            {
                Icon = value.Weather[0].Icon,
                Description = value.Weather[0].Description,
                Temp = value.Main.Temp,
                Month = value.DtTxt.ToString("dd MMM, h:mm tt"),
            })
            .ToList();
        return forecast;
    }

    private IEnumerable<ForecastDto> GetHourlyForecast(ApiForecastResponse response)
    {
        var forecast = response.List.Select(value => new ForecastDto
            {
                Icon = value.Weather[0].Icon,
                Description = value.Weather[0].Description,
                Temp = value.Main.Temp,
                Month = value.DtTxt.ToString("dd MMM, h:mm tt"),
            })
            .ToList();
        return forecast;
    }
    
    private async Task<ApiForecastResponse> GetWeatherForecastAsync(long locationId, long userId)
    {
        try
        {
            var location = await (_context.Locations ?? throw new InvalidOperationException())
                .FirstOrDefaultAsync(location => location.LocationId == locationId && location.UserId == userId);
            
            if (location is null) throw new UserNotFoundException();
            
            var lon = location.Longitude;
            var lat = location.Latitude;
            var apiKey = _config["Weather:ServiceApiKey"];
            
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Path = "data/2.5/forecast",
                Query = $"lat={lat}&lon={lon}&appid={apiKey}&units=metric",
            };
            var requestUri = uriBuilder.Uri;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var forecast = Newtonsoft.Json.JsonConvert
                .DeserializeObject<ApiForecastResponse>(jsonString);
            
            return forecast;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new OpenWeatherApiException();
        }
    }
}
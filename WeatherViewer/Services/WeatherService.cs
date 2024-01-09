using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.Exceptions;
using WeatherViewer.Models;
using WeatherViewer.Models.API;
using WeatherViewer.Models.DTOs;
using WeatherViewer.Services.Interfaces;

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
    
    public async Task<IEnumerable<ApiLocationResponse>> SearchLocationsAsync(string name)
    {
        try
        {
            var apiKey = _config["Weather:ServiceApiKey"];
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Path = "geo/1.0/direct",
                Query = $"q={name}" +
                        $"&appid={apiKey}" +
                        $"&limit={Limit}",
            };
            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            var foundLocations = Newtonsoft.Json.JsonConvert
                .DeserializeObject<IEnumerable<ApiLocationResponse>>(jsonString);
            return foundLocations!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new OpenWeatherApiException();
        }
    }

    public async Task AddLocationAsync(LocationDto request, long userId)
    {
        // create location
        Location location = new()
        {
            UserId = userId,
            Name = request.Name,
            Latitude = request.Lat,
            Longitude = request.Lon,
        };
        await (_context.Locations ?? throw new InvalidOperationException())
            .AddAsync(location);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLocationAsync(long locationId, long userId)
    {
        var location = await GetLocationAsync(userId, locationId);
        (_context.Locations ?? throw new InvalidOperationException()).Remove(location);
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

    public async Task<IEnumerable<ForecastDto>> GetWeatherForecastsAsync(long locationId, long userId)
    {
        var response = await GetForecastResponseAsync(locationId, userId);
        var forecast = response.Forecasts.Select(value => new ForecastDto
            {
                Icon = value.Weather[0].Icon,
                Description = value.Weather[0].Description,
                Temp = value.Main.Temp,
                Month = value.DtTxt.ToString("dd MMM, h:mm tt"),
            })
            .ToList();
        return forecast;
    }

    public async Task<IEnumerable<WeatherDto>> GetWeatherAsync(long userId)
    {
        var weather = new List<WeatherDto>();
        
        // get user locations
        var locations = await GetLocationsAsync(userId);
        foreach (var location in locations)
        {
            var response = await GetWeatherForLocationAsync(location);
            weather.Add(new WeatherDto()
            {
                Id = location.LocationId,
                Name = response.Name,
                Country = response.Sys.Country,
                Temp = response.Main.Temp,
                TempMin = response.Main.TempMin,
                TempMax = response.Main.TempMax,
                Description = response.Weather[0].Description,
                Humidity = response.Main.Humidity,
                Visibility = response.Visibility,
                FeelsLike = response.Main.FeelsLike,
                Icon = response.Weather[0].Icon,
                Speed = response.Wind.Speed,
            });
        }
        return weather;
    }
    
    private async Task<ApiWeatherResponse> GetWeatherForLocationAsync(Location location)
    {
        try
        {
            var apiKey = _config["Weather:ServiceApiKey"];
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Path = "data/2.5/weather",
                Query = $"lat={location.Latitude}" +
                        $"&lon={location.Longitude}" +
                        $"&appid={apiKey}" +
                        $"&units=metric",
            };
            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var weather = Newtonsoft.Json.JsonConvert
                .DeserializeObject<ApiWeatherResponse>(jsonString);
            return weather!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new OpenWeatherApiException();
        }
    }
    
    private async Task<ApiForecastResponse> GetForecastResponseAsync(long locationId, long userId)
    {
        try
        {
            var location = await GetLocationAsync(userId, locationId);
            var apiKey = _config["Weather:ServiceApiKey"];
            
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Path = "data/2.5/forecast",
                Query = $"lat={location.Latitude}" +
                        $"&lon={location.Longitude}" +
                        $"&appid={apiKey}" +
                        $"&units=metric",
            };
            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            var forecast = Newtonsoft.Json.JsonConvert
                .DeserializeObject<ApiForecastResponse>(jsonString);
            
            return forecast!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new OpenWeatherApiException();
        }
    }
    
    private async Task<IEnumerable<Location>> GetLocationsAsync(long userId) 
        => await (_context.Locations ?? throw new InvalidOperationException())
            .Where(location => location.UserId == userId)
            .ToListAsync();
    
    private async Task<Location?> GetLocationAsync(long userId, long locationId) =>
        await (_context.Locations ?? throw new InvalidOperationException())
            .FirstOrDefaultAsync(l => l.LocationId == locationId && l.UserId == userId) 
        ?? throw new LocationNotFoundException();
}
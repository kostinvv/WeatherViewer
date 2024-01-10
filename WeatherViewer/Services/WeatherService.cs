using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WeatherViewer.Data;
using WeatherViewer.Exceptions;
using WeatherViewer.Extensions;
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
    private readonly IDistributedCache _cache;

    public WeatherService(
        IHttpClientFactory httpClientFactory, 
        ApplicationDbContext context, 
        ILogger<WeatherService> logger, 
        IConfiguration config,
        IDistributedCache cache
        )
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _logger = logger;
        _config = config;
        _cache = cache;
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
        var location = await _context.Locations
            .FirstOrDefaultAsync(location => 
                location.UserId == userId 
                && request.Lat == location.Latitude 
                && request.Lon == location.Longitude);

        if (location is null)
        {
            location = new Location
            {
                UserId = userId,
                Name = request.Name,
                Latitude = request.Lat,
                Longitude = request.Lon,
            };
        
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteLocationAsync(long locationId, long userId)
    {
        var location = await GetLocationAsync(userId, locationId);
        _context.Locations.Remove(location!);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllLocationsAsync(long userId)
    {
        var locations = await _context.Locations
            .Where(location => location.UserId == userId)
            .ToListAsync();
        
        _context.Locations.RemoveRange(locations);
        
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ForecastDto>> GetWeatherForecastsAsync(long locationId, long userId)
    {
        var location = await GetLocationAsync(userId, locationId);

        var key = $"Forecast_{location.Name}_{location.Longitude}_{location.Latitude}";
        var response = await _cache.GetRecordAsync<ApiForecastResponse>(key);

        if (response is null)
        {
            response = await GetForecastResponseAsync(location);
            await _cache.SetRecordAsync(key, response);
        }
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
            var key = $"{location.Name}_{location.Longitude}_{location.Latitude}";
            var response = await _cache.GetRecordAsync<ApiWeatherResponse>(key);
            
            if (response is null)
            {
                response = await GetWeatherForLocationAsync(location);
                await _cache.SetRecordAsync(key, response);
            }
            
            weather.Add(new WeatherDto
            {
                Id = location.LocationId,
                Name = location.Name,
                WeatherStation = response.Name,
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
    
    private async Task<ApiForecastResponse> GetForecastResponseAsync(Location location)
    {
        try
        {
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
        => await _context.Locations
            .Where(location => location.UserId == userId)
            .ToListAsync();
    
    private async Task<Location?> GetLocationAsync(long userId, long locationId) =>
        await _context.Locations
            .FirstOrDefaultAsync(l => l.LocationId == locationId && l.UserId == userId) 
        ?? throw new LocationNotFoundException();
}
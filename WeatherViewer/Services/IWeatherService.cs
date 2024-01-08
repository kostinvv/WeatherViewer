using WeatherViewer.DTOs;
using WeatherViewer.DTOs.Api;
using WeatherViewer.Models;

namespace WeatherViewer.Services;

public interface IWeatherService
{
    public Task<IEnumerable<FoundLocationDto>> SearchLocationsAsync(string name);

    public Task<IEnumerable<ForecastDto>> GetWeatherForecasts(long locationId, long userId);
    
    public Task AddLocationAsync(LocationRequestDto requestDto, long userId);
    
    public Task<WeatherDto> GetWeatherForLocationAsync(Location location);
    
    public Task<IEnumerable<Location>> GetLocationsAsync(long userId);
    
    public Task DeleteLocationAsync(long locationId, long userId);
    
    public Task DeleteAllLocationsAsync(long userId);
}
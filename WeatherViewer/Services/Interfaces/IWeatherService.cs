using WeatherViewer.Exceptions;
using WeatherViewer.Models.API;
using WeatherViewer.Models.DTOs;

namespace WeatherViewer.Services.Interfaces;

/// <summary> Service responsible for working with OpenWeatherAPI. </summary>
public interface IWeatherService
{
    /// <summary> Asynchronously searches for locations using the OpenWeatherMap API based on the provided name.</summary>
    /// <param name="name">The name of the location to search for.</param>
    /// <returns> An enumerable collection of <see cref="ApiLocationResponse"/> representing the locations
    /// found based on the provided name. </returns>
    /// <exception cref="OpenWeatherApiException">
    /// Thrown when an error occurs while interacting with the OpenWeatherMap API.
    /// </exception>
    public Task<IEnumerable<ApiLocationResponse>> SearchLocationsAsync(string name);
    
    /// <summary> Asynchronously adds a new location to the database for the specified user. </summary>
    /// <param name="request">The data object for the location (<see cref="LocationDto"/>),
    /// containing information about the location to be added.</param>
    /// <param name="userId"> User id. </param>
    public Task AddLocationAsync(LocationDto request, long userId);
    
    /// <summary> Asynchronously deletes a location from the database based on the location id and user id. </summary>
    /// <param name="locationId"> Location id. </param>
    /// <param name="userId"> User id. </param>
    public Task DeleteLocationAsync(long locationId, long userId);
    
    /// <summary> Asynchronously deletes all locations associated with the specified user from the database. </summary>
    /// <param name="userId"> User id. </param>
    public Task DeleteAllLocationsAsync(long userId);
    
    /// <summary> Asynchronously retrieves weather forecasts for a specified location and user. </summary>
    /// <param name="locationId"> Location id. </param>
    /// <param name="userId"> User id. </param>
    /// <returns> An enumerable collection of <see cref="ForecastDto"/>
    /// representing the weather forecasts for the specified location. </returns>
    public Task<IEnumerable<ForecastDto>> GetWeatherForecastsAsync(long locationId, long userId);
    
    /// <summary>
    /// Asynchronously retrieves current weather information for all locations associated with the specified user.
    /// </summary>
    /// <param name="userId"> User id. </param>
    /// <returns>An enumerable collection of <see cref="WeatherDto"/> representing
    /// the current weather information for each user location.</returns>
    public Task<IEnumerable<WeatherDto>> GetWeatherAsync(long userId);
}
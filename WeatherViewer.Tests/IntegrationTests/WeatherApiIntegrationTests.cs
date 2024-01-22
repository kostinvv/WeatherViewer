using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;

namespace WeatherViewer.Tests.IntegrationTests;

public class WeatherApiIntegrationTests 
    : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly TestingWebAppFactory<Program> _factory;
    private readonly HttpClient _client;

    public WeatherApiIntegrationTests(TestingWebAppFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
        });
    }
    
    [Fact]
    public async Task GetLocationsByName_ReturnsLocations()
    {
        // arrange
        var handlerMoq = new MockHttpMessageHandler();
        handlerMoq.When("http://api.openweathermap.org/geo/1.0/*")
            .Respond("application/json", Utilities.GetLocationsJsonString());

        var webHost = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => new HttpClient(handlerMoq));
            });
        });
        
        using var scope = webHost.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();
        
        // act
        var result = await service.SearchLocationsAsync("Moscow");

        // assert
        Assert.Equal(2, result.ToList().Count);
    }

    [Fact]
    public async Task GetLocationsByName_ReturnsOpenWeatherApiException()
    {
        var handlerMoq = new MockHttpMessageHandler();
        
        // no response
        handlerMoq.When("*");
        
        var webHost = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => new HttpClient(handlerMoq));
            });
        });
        
        using var scope = webHost.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();

        // act
        var openWeatherApiEx = await Assert.ThrowsAsync<OpenWeatherApiException>(async () 
            => await service.SearchLocationsAsync("Moscow"));
        
        // assert
        Assert.Contains("No response.", openWeatherApiEx.Message);
    }

    [Fact]
    public async Task Post_AddLocationAnonymousUser_ReturnsRedirectToLogin()
    {
        var formData = new FormUrlEncodedContent(Utilities.LocationFormData);
        
        var response = await _client.PostAsync($"/Home/AddLocation", formData);
        
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/user/login", response.Headers.Location?.OriginalString, ignoreCase: true);
    }

    [Fact] 
    public async Task Post_AddLocationForTheUser_ReturnsLocations()
    {
        var sessionId = Guid.NewGuid().ToString();
        var formData = new FormUrlEncodedContent(Utilities.LocationFormData);

        var webHost = _factory.WithWebHostBuilder(_ => { });
        var client = webHost.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
        });

        using var scope = webHost.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        await cache.SetStringAsync(sessionId, "1");
        client.DefaultRequestHeaders.Add("Cookie", $"SessionId={sessionId}");
        
        var response = await client.PostAsync($"/Home/AddLocation", formData);
        var locations = dbContext.Locations.Where(location => location.UserId == 1).ToList();
        
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/", response.Headers.Location?.OriginalString, ignoreCase: true);
        Assert.Single(locations);
        Assert.Equal(1, locations[0].UserId);
        Assert.Equal("Moscow", locations[0].Name);
    }

    [Fact]
    public async Task GetWeatherForUserLocation_ReturnsWeather()
    {
        // arrange
        var handlerMoq = new MockHttpMessageHandler();
        handlerMoq.When("http://api.openweathermap.org/data/2.5/*")
            .Respond("application/json", Utilities.GetWeatherJsonString());

        var webHost = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => new HttpClient(handlerMoq));
            });
        });
        
        using var scope = webHost.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();

        // act
        
        await context.Locations.AddAsync(new Location {
            UserId = 1, Latitude = 55.7504461, Longitude = 37.6174943, Name = "Moscow" });
        await context.SaveChangesAsync();
        
        var weather = await service.GetWeatherAsync(1);
        var weatherList = weather.ToList();
        
        // assert
        Assert.Single(weatherList);
        Assert.Equal("few clouds", weatherList[0].Description);
        Assert.Equal("Moscow", weatherList[0].Name);
    }
    
    [Fact]
    public async Task GetWeatherForUserLocation_ReturnsOpenWeatherApiException()
    {
        // arrange
        var handlerMoq = new MockHttpMessageHandler();
        
        // No response
        handlerMoq.When("*");

        var webHost = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => new HttpClient(handlerMoq));
            });
        });
        using var scope = webHost.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Locations.AddAsync(new Location
        {
            Name = "Moscow", Longitude = 1, Latitude = 1, UserId = 1,
        });
        await context.SaveChangesAsync();
        
        // act
        var openWeatherApiEx = await Assert.ThrowsAsync<OpenWeatherApiException>(async () 
            => await service.GetWeatherAsync(1));
        
        // assert
        Assert.IsType<OpenWeatherApiException>(openWeatherApiEx);
        Assert.Contains("No response.", openWeatherApiEx.Message);
    }
    
    [Fact]
    public async Task Post_DeleteAnotherUserLocation_ReturnsLocationNotFoundException()
    {
        // arrange
        var webHost = _factory.WithWebHostBuilder(_ => { });
        using var scope = webHost.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();
        
        // act
        var locationNotFoundEx = await Assert.ThrowsAsync<LocationNotFoundException>(async () 
            => await service.DeleteLocationAsync(1, 2));
        
        // assert
        Assert.IsType<LocationNotFoundException>(locationNotFoundEx);
        Assert.Contains("Location not found.", locationNotFoundEx.Message);
    }
}
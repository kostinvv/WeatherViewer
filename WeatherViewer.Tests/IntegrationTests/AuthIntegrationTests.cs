namespace WeatherViewer.Tests.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly TestingWebAppFactory<Program> _factory;

    public AuthIntegrationTests(TestingWebAppFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Post_RegisterValidModel_ReturnsRedirectToAction()
    {
        // arrange
        var webHost = _factory.WithWebHostBuilder(_ => { });

        using var scope = webHost.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var client = webHost.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        
        var formData = new FormUrlEncodedContent(Utilities.RegisterValidFormData);

        // act
        var response = await client.PostAsync("/User/Register", formData);
        
        var foundUser = await context.Users
            .FirstOrDefaultAsync(user => user.Login == "User");
        
        // assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("User", foundUser.Login);
    }

    [Fact]
    public async Task Post_RegisterInvalidModel_ReturnsOkAndDisplayValidationSummaryErrors()
    {
        // arrange
        var webHost = _factory.WithWebHostBuilder(_ => { });
        var client = webHost.CreateClient();
        var formData = new FormUrlEncodedContent(Utilities.RegisterInValidFormData);

        // act
        var response = await client.PostAsync("/User/Register", formData);
        var content = await response.Content.ReadAsStringAsync();
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<li>The Login field is required.</li>", content);
        Assert.Contains("<li>Password is too easy.</li>", content);
    }
    
    [Fact]
    public async Task Post_RegisterInvalidModel_ReturnsOkAndDisplayUserExistsSummaryErrors()
    {
        // arrange
        var client = _factory.WithWebHostBuilder(_ => { }).CreateClient();
        
        var formData = new FormUrlEncodedContent(Utilities.ExistingUserFormData);
        
        // act
        var response = await client.PostAsync("/User/Register", formData);
        var content = await response.Content.ReadAsStringAsync();
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<li>A user with this login already exists.</li>", content);
    }

    [Fact]
    public async Task Post_LoginValidModel_ReturnsRedirectToActionAndCreateCookie()
    {
        // arrange
        var webHost = _factory.WithWebHostBuilder(_ => { });
        var client = webHost.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        var formData = new FormUrlEncodedContent(Utilities.ExistingUserFormData);
        
        // act
        var response = await client.PostAsync("/User/Login", formData);
        var cookieHeaders = response.Headers.GetValues("Set-Cookie").ToList();
        
        // assert
        Assert.Contains("SessionId=", cookieHeaders.First());
        Assert.Contains("weather_login=TestLogin;", cookieHeaders.Last());
    }
}
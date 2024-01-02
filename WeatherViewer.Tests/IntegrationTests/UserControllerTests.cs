using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WeatherViewer.Tests.IntegrationTests;

public class UserControllerTests : IClassFixture<TestingWebAppFactory>
{
    private readonly HttpClient _client;

    public UserControllerTests(TestingWebAppFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    
    [Fact]
    public async Task POST_Register_ValidModel_RedirectsToLogin()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "Login", "TestLogin" },
            { "Email", "test@mail.com" },
            { "Password", "TestPassword123" }
        };
        
        // Act
        var response = await _client.PostAsync("/User/Register", new FormUrlEncodedContent(formData));
        
        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/user/login", response.Headers.Location?.OriginalString, ignoreCase: true);
    }

    [Fact]
    public async Task POST_Login_ValidModel_RedirectsToHome()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "Login", "user_test_login" },
            { "Password", "TestPassword123" }
        };
        
        // Act
        var response = await _client.PostAsync("/User/Login", new FormUrlEncodedContent(formData));

        var cookies = response.Headers
            .GetValues("Set-Cookie")
            .FirstOrDefault(cookie => cookie.StartsWith("SessionId"));
        
        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/", response.Headers.Location?.OriginalString, ignoreCase: true);
        Assert.Contains("SessionId=", cookies);
    }
}
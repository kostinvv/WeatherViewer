using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeatherViewer.Data;
using WeatherViewer.DTOs;
using WeatherViewer.Exceptions;
using WeatherViewer.Models;
using WeatherViewer.Services;
using Xunit;

namespace WeatherViewer.Tests.IntegrationTests;

public class AuthServiceTests : IClassFixture<TestingWebAppFactory>
{
    private readonly TestingWebAppFactory _factory;
    
    public AuthServiceTests(TestingWebAppFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task RegisterValidModelCreateUser()
    {
        // Arrange
        var model = new RegisterRequestDto()
        {
            Email = "test@mail.ru",
            Login = "User",
            Password = "testpass",
        };

        User user = new();
        
        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            await authService.CreateUserAsync(model);
            
            if (context.Users != null)
                user = await context.Users.FirstOrDefaultAsync(entity => entity.Login == model.Login);
        }
        
        // Assert
        Assert.NotNull(user);
        Assert.Equal("User", user.Login);
    }
    
    [Fact]
    public async Task RegisterInvalidModelUserExistsException()
    {
        // Arrange
        var model = new RegisterRequestDto()
        {
            Email = "test@mail.ru",
            Login = "UserTest",
            Password = "testpass",
        };
        string message;
        
        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            if (context.Users != null)
                await context.Users.AddAsync(new User()
                {
                    Email = "test@mail.ru",
                    Login = "UserTest",
                    Password = BCrypt.Net.BCrypt.HashPassword("testpass"),
                });

            await context.SaveChangesAsync();
            
            var ex = await Assert.ThrowsAsync<UserExistsException>(async () =>
            {
                await authService.CreateUserAsync(model);
            });

            message = ex.Message;
        }

        // Assert
        Assert.Equal("A user with this login already exists.", message);
    }

    [Fact]
    public async Task LoginInvalidModelUserNotFoundException()
    {
        // Arrange
        var model = new LoginRequestDto()
        {
            Login = "test",
            Password = "testpass",
        };
        string message;

        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            
            var ex = await Assert.ThrowsAsync<UserNotFoundException>(async () =>
            {
                await authService.AuthAsync(model);
            });

            message = ex.Message;
        }

        // Assert
        Assert.Equal("The specified user doesn't exist.", message);
    }
    
    [Fact]
    public async Task LoginInvalidModelInvalidPasswordException()
    {
        // Arrange
        var loginModel = new LoginRequestDto() 
        {
            Login = "test_user",
            Password = "testpass",
        };

        var registerModel = new RegisterRequestDto()
        {
            Login = "test_user",
            Password = "testpass1",
            Email = "test@mail.ru",
        };

        string message;

        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            await authService.CreateUserAsync(registerModel);
        
            var ex = await Assert.ThrowsAsync<InvalidPasswordException>(async () =>
            {
                await authService.AuthAsync(loginModel);
            });

            message = ex.Message;
        }

        // Assert
        Assert.Equal("Invalid password.", message);
    }
    
    [Fact]
    public async Task LoginValidModelCreateAndDeleteSession()
    {
        // Arrange
        var loginModel = new LoginRequestDto() 
        {
            Login = "Test",
            Password = "Pass",
        };
        
        var registerModel = new RegisterRequestDto()
        {
            Login = "Test",
            Password = "Pass",
            Email = "test@mail.ru",
        };

        Session session;
        string message;
        
        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            
            await authService.CreateUserAsync(registerModel);
            session = await authService.AuthAsync(loginModel);
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            var ex = await Assert.ThrowsAsync<SessionNotFoundException>(async () =>
            {
                await authService.DeleteSessionAsync(session.SessionId.ToString());
            });

            message = ex.Message;
        }

        // Assert
        Assert.NotNull(session);
        Assert.Equal("Session not found.", message);
    }
}
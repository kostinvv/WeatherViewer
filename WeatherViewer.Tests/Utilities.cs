namespace WeatherViewer.Tests;

public static class Utilities
{
    public static void ReinitializeDbForTests(ApplicationDbContext context)
    {
        context.Users.RemoveRange(context.Users);
        InitializeDbForTests(context);
    }
    
    private static void InitializeDbForTests(ApplicationDbContext context)
    {
        context.Users.AddRange(GetSeedingUsers());
        context.SaveChanges();
    }

    private static List<User> GetSeedingUsers() => new ()
    {
        new User
        {
            Login = "TestLogin", 
            Password = BCrypt.Net.BCrypt.HashPassword("12345678Qw1!") 
        },
        new User
        {
            Login = "user2", 
            Password = BCrypt.Net.BCrypt.HashPassword("12345678Qw1!") 
        }
    };

    public static string GetLocationsJsonString() 
        => """
           [
               {
                   "name": "Moscow",
                   "local_names": {
                       "ru": "Москва",
                   },
                   "lat": 55.7504461,
                   "lon": 37.6174943,
                   "country": "RU",
                   "state": "Moscow"
               },
               {
                   "name": "Moscow",
                   "local_names": {
                       "en": "Moscow",
                       "ru": "Москва"
                   },
                   "lat": 46.7323875,
                   "lon": -117.0001651,
                   "country": "US",
                   "state": "Idaho"
               }
           ]
           """;
    
    public static string GetWeatherJsonString() 
        => """
           {
               "coord": {
                   "lon": 37.6171,
                   "lat": 55.7483
               },
               "weather": [
                   {
                       "id": 801,
                       "main": "Clouds",
                       "description": "few clouds",
                       "icon": "02d"
                   }
               ],
               "base": "stations",
               "main": {
                   "temp": -5.83,
                   "feels_like": -12.53,
                   "temp_min": -6.31,
                   "temp_max": -5.66,
                   "pressure": 1032,
                   "humidity": 83,
                   "sea_level": 1032,
                   "grnd_level": 1013
               },
               "visibility": 10000,
               "wind": {
                   "speed": 5.49,
                   "deg": 205,
                   "gust": 16.02
               },
               "clouds": {
                   "all": 23
               },
               "dt": 1705927524,
               "sys": {
                   "type": 2,
                   "id": 2000314,
                   "country": "RU",
                   "sunrise": 1705902077,
                   "sunset": 1705930810
               },
               "timezone": 10800,
               "id": 524901,
               "name": "Moscow",
               "cod": 200
           }
           """;
    
    public static readonly Dictionary<string, string> RegisterValidFormData = new()
    {
        { "Login", "User" }, { "Password", "Test123!" }
    };

    public static readonly Dictionary<string, string> RegisterInValidFormData = new()
    {
        { "Login", "" }, { "Password", "1" }
    };
    
    public static readonly Dictionary<string, string> ExistingUserFormData  = new()
    {
        { "Login", "TestLogin" }, { "Password", "12345678Qw1!" }
    };
    
    public static readonly Dictionary<string, string> LocationFormData  = new()
    {
        { "Name", "Moscow" }, { "Lat", "55.7504461" }, { "Lon", "37.6174943" }
    };
}
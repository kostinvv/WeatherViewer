using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WeatherViewer.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task SetRecordAsync<T>(
        this IDistributedCache cache, 
        string key,
        T data,
        TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(60),
            SlidingExpiration = unusedExpireTime
        };

        var jsonString = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(key, value: jsonString, options);
    }

    public static async Task<T?> GetRecordAsync<T>(
        this IDistributedCache cache, 
        string key)
    {
        var jsonString = await cache.GetStringAsync(key);
        
        return jsonString is null ? default : JsonSerializer.Deserialize<T>(jsonString);
    }
}
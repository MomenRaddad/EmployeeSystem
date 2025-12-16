using EmployeeSystem.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace EmployeeSystem.Infrastructure.Caching;

public class RedisAppCache(
    IDistributedCache cache,
    ILogger<RedisAppCache> logger
) : IAppCache
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cached = await cache.GetStringAsync(key);

            if (cached == null)
            {
                logger.LogInformation("[CACHE] MISS for key {Key}", key);
                return default;
            }

            logger.LogInformation("[CACHE] HIT for key {Key}", key);
            return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CACHE] Error retrieving key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpiration = null)

    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(5)
            };

            await cache.SetStringAsync(key, json, options);

            logger.LogInformation(
                "[CACHE] SET key {Key} with TTL {TTL}",
                key,
                options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CACHE] Error setting key {Key}", key);
        }
    }

    public Task RemoveAsync(string key)
    {
        logger.LogInformation("[CACHE] REMOVE key {Key}", key);
        return cache.RemoveAsync(key);
    }
}

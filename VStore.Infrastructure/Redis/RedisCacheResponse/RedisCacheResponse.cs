using System.Text.Json;
using StackExchange.Redis;
using VStore.Application.Abstractions.RedisCartService;

namespace VStore.Infrastructure.Redis.RedisCacheResponse;

public class RedisCacheResponse : IRedisCacheResponse
{
    private readonly IDatabase _redis;

    public RedisCacheResponse(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase(1);
    }

    public async Task<string?> GetCacheResponseAsync(string key)
    {
        var data = await _redis.StringGetAsync(key);
        return data.IsNullOrEmpty ? null : data.ToString();
    }

    public async Task SetCacheResponseAsync(string key, object value, TimeSpan timeToLive)
    {
        var jsonOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var data = JsonSerializer.Serialize(value, jsonOption);
        await _redis.StringSetAsync(key, data, timeToLive);
    }

    public Task<bool> DeleteCacheResponseAsync(string key)
    {
        throw new NotImplementedException();
    }
}
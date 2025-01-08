using System.Text.Json;
using StackExchange.Redis;
using VStore.Application.Abstractions.RedisCartService;

namespace VStore.Infrastructure.Redis.RedisCacheResponse;

public class RedisCacheResponse(IConnectionMultiplexer redis) : IRedisCacheResponse
{
    private readonly IDatabase _redis = redis.GetDatabase(1);

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

    public async Task DeleteCacheResponseAsync(string pattern)
    {
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(database: 1, pattern: $"*{pattern}*").ToArray();
        if (keys.Length != 0)
        {
            foreach (var k in keys)
            {
                if (ValidatePattern(k, pattern))
                {
                    await _redis.KeyDeleteAsync(k);
                }
            }
        }
    }

    private bool ValidatePattern(RedisKey redisKey, string pattern)
    {
        var key = redisKey.ToString().Split("/");
        var patternKey = pattern.Split("/");
        if (key.Length != patternKey.Length)
        {
            return false;
        }

        return true;
    }
}
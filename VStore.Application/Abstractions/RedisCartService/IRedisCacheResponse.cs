namespace VStore.Application.Abstractions.RedisCartService;

public interface IRedisCacheResponse
{
    Task<string?> GetCacheResponseAsync(string key);
    Task SetCacheResponseAsync(string key, object value, TimeSpan timeToLive);
    Task<bool> DeleteCacheResponseAsync(string key);
}
using VStore.Application.Models.RedisService;
using VStore.Domain.Entities;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.RedisCartService;

public interface IRedisCartService
{
    Task<RedisCart?> GetCartAsync(string key);
    Task<RedisCart?> SetCartAsync(AddRedisCartModel cart);
    Task<bool> DeleteCartAsync(string key);
}
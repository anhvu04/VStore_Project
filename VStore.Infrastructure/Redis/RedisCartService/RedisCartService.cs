using System.Text.Json;
using StackExchange.Redis;
using VStore.Application.Abstractions.RedisCartService;
using VStore.Application.Models.RedisService;
using VStore.Domain.Entities;

namespace VStore.Infrastructure.Redis.RedisCartService;

public class RedisCartService(IConnectionMultiplexer redis) : IRedisCartService
{
    private readonly IDatabase _redis = redis.GetDatabase();

    public async Task<RedisCart?> GetCartAsync(string key)
    {
        var data = await _redis.StringGetAsync(key);
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<RedisCart>(data!);
    }

    public async Task<RedisCart?> SetCartAsync(AddRedisCartModel cartModel)
    {
        var cart = await GetCartAsync(cartModel.Id);
        if (cart is null)
        {
            cart = new RedisCart
            {
                Id = cartModel.Id,
                RedisCartItems = new List<RedisCartItem>()
                {
                    new()
                    {
                        ProductId = cartModel.ProductId,
                        Quantity = cartModel.Quantity
                    }
                }
            };
        }
        else
        {
            var isExistProduct = cart.RedisCartItems.FirstOrDefault(x => x.ProductId == cartModel.ProductId);
            if (isExistProduct is not null)
            {
                isExistProduct.Quantity += cartModel.Quantity;
            }
            else
            {
                cart.RedisCartItems.Add(new RedisCartItem
                {
                    ProductId = cartModel.ProductId,
                    Quantity = cartModel.Quantity
                });
            }
        }

        // Set the cart in Redis with an expiration of 30 days
        var created = await _redis.StringSetAsync(cartModel.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        return created ? cart : null;
    }

    public async Task<bool> DeleteCartAsync(string key)
    {
        return await _redis.KeyDeleteAsync(key);
    }
}
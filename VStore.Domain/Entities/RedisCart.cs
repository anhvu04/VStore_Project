using System.Text.Json.Serialization;

namespace VStore.Domain.Entities;

public class RedisCart
{
    public required string Id { get; set; }
    public List<RedisCartItem> RedisCartItems { get; set; }
}
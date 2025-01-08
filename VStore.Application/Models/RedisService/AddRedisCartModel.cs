using VStore.Domain.Entities;

namespace VStore.Application.Models.RedisService;

public class AddRedisCartModel
{
    public required string Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
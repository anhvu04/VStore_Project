namespace VStore.Domain.Entities;

public class RedisCartItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Abstractions.RedisCartService;
using VStore.Application.Models.RedisService;
using VStore.Application.Usecases.Cart.Command.AddToCart;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Entities;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[Route("api/redis-cart")]
[ApiController]
public class RedisCartController(ISender sender, IRedisCartService redisCartService) : ApiController(sender)
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetRedisCart()
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                     Guid.Empty.ToString();

        var cart = await redisCartService.GetCartAsync(userId);
        return cart is not null
            ? Ok(cart)
            : Ok(new RedisCart
            {
                Id = userId,
                RedisCartItems = []
            });
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> SetRedisCart([FromBody] AddToCartCommand cartModel)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                     Guid.Empty.ToString();
        var cart = new AddRedisCartModel
        {
            Id = userId,
            ProductId = cartModel.ProductId,
            Quantity = cartModel.Quantity
        };
        var res = await redisCartService.SetCartAsync(cart);
        return res is not null
            ? Ok(res)
            : BadRequest("Failed to set cart");
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> DeleteRedisCart([FromQuery] string id)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                     Guid.Empty.ToString();
        if (userId != id)
        {
            return BadRequest("Invalid cart id");
        }

        var res = await redisCartService.DeleteCartAsync(id);
        return res
            ? Ok()
            : BadRequest("Failed to delete cart");
    }
}
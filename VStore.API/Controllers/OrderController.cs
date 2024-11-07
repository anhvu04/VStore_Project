using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Order.Command.CancelOrderAdmin;
using VStore.Application.Usecases.Order.Command.UpdateOrderStatus;
using VStore.Application.Usecases.Order.Query.GetOrder;
using VStore.Application.Usecases.Order.Query.GetOrders;
using VStore.Domain.Enums;
using AuthenticationScheme = VStore.Domain.AuthenticationScheme.AuthenticationScheme;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(ISender sender) : ApiController(sender)
{
    [HttpGet("{orderId}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var query = new GetOrderQuery(orderId, Guid.Empty);
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query)
    {
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPatch("{orderId}/status")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusCommand command)
    {
        command = command with { OrderId = orderId };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPost("{orderId}/cancel")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        var command = new CancelOrderAdminCommand(orderId);
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }
}
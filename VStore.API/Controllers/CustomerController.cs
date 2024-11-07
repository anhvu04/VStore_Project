using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Usecases.Order.Query.GetOrder;
using VStore.Application.Usecases.Order.Query.GetOrders;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController(ISender sender) : ApiController(sender)
{
    #region Customer

    [HttpGet("orders/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetCustomerOrders(Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        var query = new GetOrderQuery(id, userId);
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("orders")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetCustomerOrders([FromQuery] GetOrdersQuery query)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        query.CustomerId = userId;
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    #endregion
}
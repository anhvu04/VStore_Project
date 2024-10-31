using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;
using VStore.Application.Usecases.CustomerAddress.Command.DeleteCustomerAddress;
using VStore.Application.Usecases.CustomerAddress.Command.UpdateCustomerAddress;
using VStore.Application.Usecases.CustomerAddress.Query.GetCustomerAddress;
using VStore.Application.Usecases.CustomerAddress.Query.GetCustomerAddresses;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/customer-address")]
public class CustomerAddressController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> CreateCustomerAddress(
        [FromBody] CreateCustomerAddressCommand addressCommand)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        addressCommand = addressCommand with { UserId = userId };
        var result = await Sender.Send(addressCommand);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetCustomerAddress()
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        var query = new GetCustomerAddressesQuery(userId);
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetCustomerAddress([FromRoute] Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        var query = new GetCustomerAddressQuery(userId, id);
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> UpdateCustomerAddress([FromRoute] Guid id,
        [FromBody] UpdateCustomerAddressCommand addressCommand)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        addressCommand = addressCommand with { UserId = userId, Id = id };
        var result = await Sender.Send(addressCommand);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> DeleteCustomerAddress([FromRoute] Guid id)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        var command = new DeleteCustomerAddressCommand(userId, id);
        var result = await Sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
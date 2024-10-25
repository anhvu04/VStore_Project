using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;
using VStore.Application.Usecases.GHNAddress.Query.GetDistrict;
using VStore.Application.Usecases.GHNAddress.Query.GetProvince;
using VStore.Application.Usecases.GHNAddress.Query.GetWard;
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
    public async Task<IActionResult> CreateCustomerAddress([FromBody] CreateCustomerAddressAddressCommand addressCommand)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        addressCommand = addressCommand with { UserId = userId };
        var result = await Sender.Send(addressCommand);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
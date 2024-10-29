using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Usecases.Checkout.Command.Checkout;
using VStore.Domain.Enums;
using AuthenticationScheme = VStore.Domain.AuthenticationScheme.AuthenticationScheme;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> Checkout([FromBody] CheckoutCommand command)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        command = command with { UserId = userId };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }
}
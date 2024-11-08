using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Usecases.Order.Query.GetOrder;
using VStore.Application.Usecases.User.Command.ChangePassword;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[ApiController]
[Route("api")]
public class UserController(ISender sender) : ApiController(sender)
{
    #region Account

    [HttpPatch("user/account/change-password")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access)]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        command.UserId = userId;
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    #endregion
}
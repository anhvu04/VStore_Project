using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Order.Command.PayOsWebHook;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebHook(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> PayOsWebHook([FromBody] PayOsWebHookCommand command)
    {
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }
}
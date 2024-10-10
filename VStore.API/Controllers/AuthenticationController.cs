using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Authentication.Command;

namespace VStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(ISender sender) : ApiController(sender)
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var res = await Sender.Send(command);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }
    }
}
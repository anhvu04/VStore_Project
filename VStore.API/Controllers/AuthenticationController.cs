using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Authentication.Command.Login;
using VStore.Application.Usecases.Authentication.Command.Register;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var res = await Sender.Send(command);
            return res.IsSuccess ? Ok() : BadRequest(res.Error);
        }
    }
}
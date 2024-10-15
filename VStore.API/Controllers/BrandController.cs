using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Brand.Command.CreateBrand;
using VStore.Application.Usecases.Brand.Command.DeleteBrand;
using VStore.Application.Usecases.Brand.Command.UpdateBrand;
using VStore.Application.Usecases.Brand.Query.GetBrand;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[Route("api/brand")]
public class BrandController(ISender sender) : ApiController(sender)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new GetBrandQuery(id);
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandCommand command)
    {
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateBrand([FromRoute] int id, [FromBody] UpdateBrandCommand command)
    {
        command.Id = id;
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteBrand([FromRoute] int id)
    {
        var command = new DeleteBrandCommand(id);
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }
}
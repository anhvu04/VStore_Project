using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Brand.Command.CreateBrand;
using VStore.Application.Usecases.Brand.Command.DeleteBrand;
using VStore.Application.Usecases.Brand.Command.UpdateBrand;
using VStore.Application.Usecases.Brand.Query.GetBrand;
using VStore.Application.Usecases.Category.Command.CreateCategory;
using VStore.Application.Usecases.Category.Command.DeleteCategory;
using VStore.Application.Usecases.Category.Command.UpdateCategory;
using VStore.Application.Usecases.Category.Query.GetCategory;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[Route("api/product")]
public class ProductController(ISender sender) : ApiController(sender)
{
    #region Brands

    [HttpGet("brands/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new GetBrandQuery(id);
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost("brands")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandCommand command)
    {
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPatch("brands/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateBrand([FromRoute] int id, [FromBody] UpdateBrandCommand command)
    {
        command.Id = id;
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpDelete("brands/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteBrand([FromRoute] int id)
    {
        var command = new DeleteBrandCommand(id);
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    #endregion

    #region Categories

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategories([FromRoute] int id)
    {
        var query = new GetCategoryQuery(id);
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost("categories")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPatch("categories/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryCommand command)
    {
        command = command with { Id = id };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpDelete("categories/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        var command = new DeleteCategoryCommand(id);
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    #endregion
}
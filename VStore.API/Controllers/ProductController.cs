using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Brand.Command.CreateBrand;
using VStore.Application.Usecases.Brand.Command.CreateBrandLogo;
using VStore.Application.Usecases.Brand.Command.DeleteBrand;
using VStore.Application.Usecases.Brand.Command.UpdateBrand;
using VStore.Application.Usecases.Brand.Query.GetBrand;
using VStore.Application.Usecases.Brand.Query.GetBrands;
using VStore.Application.Usecases.Category.Command.CreateCategory;
using VStore.Application.Usecases.Category.Command.DeleteCategory;
using VStore.Application.Usecases.Category.Command.UpdateCategory;
using VStore.Application.Usecases.Category.Query.GetCategories;
using VStore.Application.Usecases.Category.Query.GetCategory;
using VStore.Application.Usecases.Product.Command.CreateProduct;
using VStore.Application.Usecases.Product.Command.CreateProductThumbnail;
using VStore.Application.Usecases.Product.Command.DeleteProduct;
using VStore.Application.Usecases.Product.Command.UpdateProduct;
using VStore.Application.Usecases.Product.Query.GetProduct;
using VStore.Application.Usecases.Product.Query.GetProducts;
using VStore.Application.Usecases.ProductImage.Command.CreateProductImage;
using VStore.Application.Usecases.ProductImage.Command.DeleteProductImage;
using VStore.Application.Usecases.ProductImage.Command.UpdateProductImage;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[Route("api/products")]
public class ProductController(ISender sender) : ApiController(sender)
{
    #region Products

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        var query = new GetProductQuery(id);
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost("{id}/thumbnail")]
    public async Task<IActionResult> CreateProductThumbnail(Guid id, IFormFile thumbnail)
    {
        var command = new CreateProductThumbnailCommand { ProductId = id, Thumbnail = thumbnail };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductCommand command)
    {
        command = command with { Id = id };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var command = new DeleteProductCommand(id);
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    #endregion

    #region Brands

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands([FromQuery] GetBrandsQuery query)
    {
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("brands/{id}")]
    public async Task<IActionResult> GetBrand([FromRoute] int id)
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

    [HttpPost("brands/{id}/logo")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CreateBrandLogo([FromRoute] int id, IFormFile logo)
    {
        var command = new CreateBrandLogoCommand { Id = id, Logo = logo };
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

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQuery query)
    {
        var res = await Sender.Send(query);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory([FromRoute] int id)
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

    #region Images

    [HttpPost("{id}/images")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> CreateProductImage(Guid id, [FromForm] List<IFormFile> images)
    {
        var command = new CreateProductImageCommand { ProductId = id, Images = images };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpPatch("product-images/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateProductImage(int id, [FromBody] UpdateProductImageCommand command)
    {
        command = command with { Id = id };
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    [HttpDelete("product-images/{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteProductImage(int id)
    {
        var command = new DeleteProductImageCommand(id);
        var res = await Sender.Send(command);
        return res.IsSuccess ? Ok() : BadRequest(res.Error);
    }

    #endregion
}
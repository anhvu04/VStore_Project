using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Abstractions.GhnService;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/ghn")]
public class GhnController(ISender sender, IGhnService ghnService) : ApiController(sender)
{
    private readonly IGhnService _ghnService = ghnService;

    [HttpGet("province")]
    public async Task<IActionResult> GetProvince()
    {
        var result = await _ghnService.GetProvince();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("district/{provinceId}")]
    public async Task<IActionResult> GetDistrict(int provinceId)
    {
        var result = await _ghnService.GetDistrict(provinceId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("ward/{districtId}")]
    public async Task<IActionResult> GetWard(int districtId)
    {
        var result = await _ghnService.GetWard(districtId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("order-info/{shippingCode}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access,
        Roles = $"{nameof(Role.Admin)}, {nameof(Role.Customer)}")]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetGhnOrderInfo(string shippingCode)
    {
        var role = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        Guid userId = Guid.Empty;
        if (role == nameof(Role.Customer))
        {
            userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        }

        var result = await _ghnService.GetShippingOrder(userId, shippingCode);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("shipping-fee/{customerAddressId}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Customer))]
    [ServiceFilter(typeof(UserExistFilter))]
    public async Task<IActionResult> GetGhnShippingFee([FromRoute] Guid customerAddressId)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        var result = await _ghnService.GetShippingFee(customerAddressId, userId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
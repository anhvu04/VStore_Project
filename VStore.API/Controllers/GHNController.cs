using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.GHNAddress.Query.GetDistrict;
using VStore.Application.Usecases.GHNAddress.Query.GetProvince;
using VStore.Application.Usecases.GHNAddress.Query.GetWard;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/ghn")]
public class GhnController(ISender sender) : ApiController(sender)
{
    [HttpGet("province")]
    public async Task<IActionResult> GetProvince()
    {
        var query = new GetProvinceQuery();
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("district/{provinceId}")]
    public async Task<IActionResult> GetDistrict(int provinceId)
    {
        var query = new GetDistrictQuery(provinceId);
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("ward/{districtId}")]
    public async Task<IActionResult> GetWard(int districtId)
    {
        var query = new GetWardQuery(districtId);
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
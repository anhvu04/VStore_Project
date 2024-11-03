using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Abstractions.VNPayService;
using VStore.Application.Models.VnPayService;

namespace VStore.API.Controllers;

[ApiController]
public class VnPayController(ISender sender, IVnPayService vnPayService) : ApiController(sender)
{
    private readonly IVnPayService _vnPayService = vnPayService;

    [HttpGet]
    [Route("IPN")]
    public async Task<IActionResult> Ipn()
    {
        var query = Request.Query;
        var result = await _vnPayService.VerifyIpnPayment(query);
        return Ok(JsonSerializer.Serialize(result));
    }
}
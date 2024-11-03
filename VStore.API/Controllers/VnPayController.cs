using System.Text.Json;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Abstractions.VNPayService;
using VStore.Application.Models.VnPayService;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/vnpay")]
public class VnPayController(ISender sender, IVnPayService vnPayService) : ApiController(sender)
{
    private readonly IVnPayService _vnPayService = vnPayService;

    [HttpGet]
    [Route("IPN")]
    public async Task<IActionResult> Ipn()
    {
        var query = Request.Query;
        var result = await _vnPayService.VerifyIpnPayment(query);
        var response = JsonSerializer.Serialize(result);
        string escapedJsonString = response.Replace("\"", "\\\"");
        return Ok($"\"{escapedJsonString}\"");
    }
}
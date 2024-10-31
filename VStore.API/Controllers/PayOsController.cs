using MediatR;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Models.PayOsService;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/payos")]
public class PayOsController(ISender sender, IPayOsService payOsService) : ApiController(sender)
{
    private readonly IPayOsService _payOsService = payOsService;

    [HttpPost("webhook_")]
    public async Task<IActionResult> VerifyPayOsWebHook([FromBody] VerifyPayOsWebHookModel model)
    {
        var res = await _payOsService.VerifyPaymentWebHook(model);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> VerifyPayOsWebHook([FromBody] WebhookType data)
    {
        
        var res = await _payOsService.VerifyPaymentWebHookType(data);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost("signature")]
    public async Task<IActionResult> CreatePayOsSignatureWebHook([FromBody] CreatePayOsSignatureModel data)
    {
        var res = await _payOsService.CreateSignatureWebHook(data);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("{orderCode}")]
    public async Task<IActionResult> GetPaymentInformation([FromRoute] long orderCode)
    {
        var res = await _payOsService.GetPaymentInformation(orderCode);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }
}
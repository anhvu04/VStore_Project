using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Models.PayOsService;
using WebhookType = Net.payOS.Types.WebhookType;

namespace VStore.API.Controllers;

[ApiController]
[Route("api/payos")]
public class PayOsController(ISender sender, IPayOsService payOsService) : ApiController(sender)
{
    private readonly IPayOsService _payOsService = payOsService;

    [HttpPost("webhook/verify")]
    public async Task<IActionResult> VerifyPayOsWebHook([FromBody] VerifyPayOsWebHookModel model)
    {
        var res = await _payOsService.VerifyPaymentWebHook(model);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> VerifyPayOsWebHook([FromBody] WebhookType data)
    {
        var webhookType = new WebhookTypeModel
        {
            Code = data.code,
            Desc = data.desc,
            Success = data.success,
            Signature = data.signature,
            Data = new WebhookDataModel
            {
                OrderCode = data.data.orderCode,
                Amount = data.data.amount,
                Description = data.data.description,
                AccountNumber = data.data.accountNumber,
                Reference = data.data.reference,
                TransactionDateTime = data.data.transactionDateTime,
                Currency = data.data.currency,
                PaymentLinkId = data.data.paymentLinkId,
                Code = data.data.code,
                Desc = data.data.desc,
                CounterAccountBankId = data.data.counterAccountBankId,
                CounterAccountBankName = data.data.counterAccountBankName,
                CounterAccountName = data.data.counterAccountName,
                CounterAccountNumber = data.data.counterAccountNumber,
                VirtualAccountName = data.data.virtualAccountName,
                VirtualAccountNumber = data.data.virtualAccountNumber
            }
        };
        var res = await _payOsService.VerifyPaymentWebHookType(webhookType);
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
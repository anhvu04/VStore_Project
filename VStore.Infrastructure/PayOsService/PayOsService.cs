using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Application.Usecases.Order.Command.PayOsWebHook;
using VStore.Application.Usecases.Order.Common;
using ItemData = Net.payOS.Types.ItemData;
using WebhookData = VStore.Application.Usecases.Order.Command.PayOsWebHook.WebhookData;

namespace VStore.Infrastructure.PayOsService;

public class PayOsService : IPayOsService
{
    private readonly PayOS _payOs;
    private readonly IConfiguration _configuration;

    public PayOsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _payOs = new PayOS(_configuration["PayOs:ClientId"]!, _configuration["PayOs:ApiKey"]!,
            _configuration["PayOs:ChecksumKey"]!);
        _payOs.confirmWebhook(_configuration["PayOs: WebhookUrl"]!);
    }

    public async Task<ApiResponseModel> CreatePaymentLink(CreatePayOsPaymentModel model)
    {
        try
        {
            var paymentData = new PaymentData(
                orderCode: model.OrderCode,
                amount: model.Amount,
                description: model.Description,
                items: model.Items.Select(i => new ItemData(i.Name, i.Quantity, i.Price)).ToList(),
                cancelUrl: _configuration["PayOs:CancelUrl"]!,
                returnUrl: _configuration["PayOs:ReturnUrl"]!,
                expiredAt: DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            );
            var payment = await _payOs.createPaymentLink(paymentData);
            return new ApiResponseModel(code: 200, data: payment.checkoutUrl);
        }
        catch (Exception e)
        {
            return new ApiResponseModel(code: 400, data: null);
        }
    }

    public Task<ApiResponseModel> VerifyPaymentWebHook(PayOsWebHookCommand data)
    {
        try
        {
            WebhookData webhookData = data.webhookData as WebhookData ?? throw new InvalidOperationException();
            var webHookData = new Net.payOS.Types.WebhookData(orderCode: webhookData.orderCode,
                amount: webhookData.amount,
                description: webhookData.description, accountNumber: webhookData.accountNumber,
                reference: webhookData.reference,
                transactionDateTime: webhookData.transactionDateTime, currency: webhookData.currency,
                paymentLinkId: webhookData.paymentLinkId, code: data.Code, desc: data.Desc,
                counterAccountBankId: webhookData.counterAccountBankId,
                counterAccountBankName: webhookData.counterAccountBankName,
                counterAccountName: webhookData.counterAccountName,
                counterAccountNumber: webhookData.counterAccountNumber,
                virtualAccountName: webhookData.virtualAccountName,
                virtualAccountNumber: webhookData.virtualAccountNumber);

            var webHookType = new WebhookType(code: data.Code, desc: data.Desc, success: data.Success,
                data: webHookData,
                signature: data.Signature);
            var validateSignature = _payOs.verifyPaymentWebhookData(webHookType);
            return Task.FromResult(new ApiResponseModel(code: 200, data: new PayOsWebHookResponse
            {
                Code = "00",
                OrderCode = validateSignature.orderCode,
            }));
        }
        catch (Exception e)
        {
            return Task.FromResult(new ApiResponseModel(code: 400, data: null));
        }
    }
}
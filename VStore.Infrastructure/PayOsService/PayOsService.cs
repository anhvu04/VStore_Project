using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Application.Usecases.Order.Command.PayOsWebHook;
using ItemData = Net.payOS.Types.ItemData;

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
        _payOs.confirmWebhook(_configuration["PayOs:WebhookUrl"]!);
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
            var webHookData = new Net.payOS.Types.WebhookData(orderCode: data.Data.orderCode, amount: data.Data.amount,
                description: data.Data.description, accountNumber: data.Data.accountNumber,
                reference: data.Data.reference,
                transactionDateTime: data.Data.transactionDateTime, currency: data.Data.currency,
                paymentLinkId: data.Data.paymentLinkId, code: data.Code, desc: data.Desc,
                counterAccountBankId: data.Data.counterAccountBankId,
                counterAccountBankName: data.Data.counterAccountBankName,
                counterAccountName: data.Data.counterAccountName, counterAccountNumber: data.Data.counterAccountNumber,
                virtualAccountName: data.Data.virtualAccountName, virtualAccountNumber: data.Data.virtualAccountNumber);

            var webHookType = new WebhookType(code: data.Code, desc: data.Desc, success: data.Success,
                data: webHookData,
                signature: data.Signature);
            var validateSignature = _payOs.verifyPaymentWebhookData(webHookType);
            return Task.FromResult(new ApiResponseModel(code: 200, data: validateSignature.code));
        }
        catch (Exception e)
        {
            return Task.FromResult(new ApiResponseModel(code: 400, data: null));
        }
    }
}
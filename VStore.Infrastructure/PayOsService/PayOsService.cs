using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;
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
    }

    public async Task<ApiResponseModel> CreatePaymentLink(CreatePaymentModel model)
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
}
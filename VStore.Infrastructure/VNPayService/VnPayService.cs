using Microsoft.Extensions.Configuration;
using VStore.Application.Abstractions.VNPayService;
using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;

namespace VStore.Infrastructure.VNPayService;

public class VnPayService : IVnPayService
{
    private readonly VnPayLibrary _vnpay;
    private readonly IConfiguration _configuration;

    public VnPayService(IConfiguration configuration, VnPayLibrary vnpay)
    {
        _configuration = configuration;
        _vnpay = vnpay;
        _vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]!);
        _vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]!);
        _vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]!);
        _vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]!);
        _vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]!);
        _vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]!);
    }

    public ApiResponseModel CreatePaymentLink(CreateVnPayPaymentModel model)
    {
        try
        {
            // Clear all request data before adding new data
            _vnpay.ClearRequestSpecificData();

            _vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            _vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_OrderInfo", model.OrderInfo);
            _vnpay.AddRequestData("vnp_OrderType", model.OrderType);
            _vnpay.AddRequestData("vnp_TxnRef", model.TxnRef.ToString());
            _vnpay.AddRequestData("vnp_IpAddr", model.IpAddr);
            string paymentUrl =
                _vnpay.CreateRequestUrl(_configuration["VnPay:PaymentUrl"]!, _configuration["VnPay:HashSecret"]!);
            return new ApiResponseModel
            {
                Code = 200,
                Data = paymentUrl
            };
        }
        catch (Exception e)
        {
            return new ApiResponseModel
            {
                Code = 400,
                Message = e.Message
            };
        }
    }
}
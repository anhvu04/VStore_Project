using Microsoft.AspNetCore.Http;
using VStore.Application.Models;
using VStore.Application.Models.VnPayService;
using VStore.Application.Usecases.Checkout.Common;

namespace VStore.Application.Abstractions.VNPayService;

public interface IVnPayService
{
    ApiResponseModel CreatePaymentLink(CreateVnPayPaymentModel model);
    Task<VnPayIpnResponse> VerifyIpnPayment(IQueryCollection request);
}
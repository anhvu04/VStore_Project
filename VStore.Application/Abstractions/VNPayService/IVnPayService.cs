using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;

namespace VStore.Application.Abstractions.VNPayService;

public interface IVnPayService
{
    ApiResponseModel CreatePaymentLink(CreateVnPayPaymentModel model);
}
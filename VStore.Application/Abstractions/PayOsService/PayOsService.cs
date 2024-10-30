using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Application.Usecases.Order.Command.PayOsWebHook;

namespace VStore.Application.Abstractions.PayOsService;

public interface IPayOsService
{
    Task<ApiResponseModel> CreatePaymentLink(CreatePayOsPaymentModel model);
    Task<ApiResponseModel> VerifyPaymentWebHook(PayOsWebHookCommand data);
}
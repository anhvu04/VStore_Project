using VStore.Application.Models;
using VStore.Application.Usecases.Checkout.Common;

namespace VStore.Application.Abstractions.PayOsService;

public interface IPayOsService
{
    Task<ApiResponseModel> CreatePaymentLink(CreatePaymentModel model);
}
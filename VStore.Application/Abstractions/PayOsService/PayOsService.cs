using VStore.Application.Models;
using VStore.Application.Models.PayOsService;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.PayOsService;

public interface IPayOsService
{
    Task<ApiResponseModel> CreatePaymentLink(CreatePayOsPaymentModel model);

    Task<Result<PayOsWebHookResponseModel>> VerifyPaymentWebHook(VerifyPayOsWebHookModel request,
        CancellationToken cancellationToken = default);

    Task<Result<PayOsWebHookResponseModel>> CreateSignatureWebHook(CreatePayOsSignatureModel request,
        CancellationToken cancellationToken = default);

    Task<Result<PayOsWebHookResponseModel>> GetPaymentInformation(long orderCode,
        CancellationToken cancellationToken = default);
}
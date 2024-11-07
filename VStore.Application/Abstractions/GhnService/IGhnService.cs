using VStore.Application.Models;
using VStore.Application.Models.GhnService;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.GhnService;

public interface IGhnService
{
    Task<ApiResponseModel> CreateShippingOrder(CreateGhnOrderModel model);
    Task<Result<GhnResponseModel>> GetShippingOrder(Guid customerId, string shippingCode);
    Task<Result<GhnResponseModel>> GetShippingFee(Guid customerAddressId, Guid customerId);
}
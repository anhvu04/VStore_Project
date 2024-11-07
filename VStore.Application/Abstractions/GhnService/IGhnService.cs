using VStore.Application.Models;
using VStore.Application.Models.GhnService;

namespace VStore.Application.Abstractions.GhnService;

public interface IGhnService
{
    Task<ApiResponseModel> CreateShippingOrder(CreateGhnShippingOrderModel model);
}
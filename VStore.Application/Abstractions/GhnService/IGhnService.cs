using VStore.Application.Models;
using VStore.Application.Models.GhnService;
using VStore.Application.Usecases.GHNExpress.Common;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.GhnService;

public interface IGhnService
{
    Task<ApiResponseModel> CreateShippingOrder(CreateGhnOrderModel model);
    Task<Result<GhnResponseModel>> GetShippingOrder(Guid customerId, string shippingCode);
    Task<Result<GhnResponseModel>> GetShippingFee(Guid customerAddressId, Guid customerId);
    Task<Result<List<GetProvinceModel>>> GetProvince();
    Task<Result<List<GetDistrictModel>>> GetDistrict(int provinceId);
    Task<Result<List<GetWardModel>>> GetWard(int districtId);
}
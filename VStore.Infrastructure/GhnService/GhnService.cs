using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VStore.Application.Abstractions.ApiService;
using VStore.Application.Abstractions.GhnService;
using VStore.Application.Models;
using VStore.Application.Models.GhnService;

namespace VStore.Infrastructure.GhnService;

public class GhnService : IGhnService
{
    private readonly IApiService _apiService;
    private readonly string _token;
    private readonly string _shopId;

    private const string CreateOrderUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";

    public GhnService(IApiService apiService, IConfiguration configuration)
    {
        _apiService = apiService;
        _token = configuration["GHNExpress:Token"]!;
        _shopId = configuration["GHNExpress:ShopId"]!;
    }

    public async Task<ApiResponseModel> CreateShippingOrder(CreateGhnShippingOrderModel model)
    {
        var result = await _apiService.PostAsync(CreateOrderUrl, model, headers =>
        {
            headers.Add("Token", _token);
            headers.Add("ShopId", _shopId);
        });
        if (!result.IsSuccess)
        {
            return new ApiResponseModel
            {
                Code = 400,
                Message = result.Value,
            };
        }

        var response = JsonSerializer.Deserialize<CreateGhnShippingOrderResponseModel>(result.Value!);
        if (response == null)
        {
            return new ApiResponseModel
            {
                Code = 400,
                Message = "Invalid response",
            };
        }

        return new ApiResponseModel
        {
            Code = response.Code,
            Message = null,
            Data = response.Data.OrderCode
        };
    }
}
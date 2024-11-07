using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VStore.Application.Abstractions.ApiService;
using VStore.Application.Abstractions.GhnService;
using VStore.Application.CoreHelper;
using VStore.Application.Models;
using VStore.Application.Models.GhnService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Infrastructure.GhnService;

public class GhnService : IGhnService
{
    private readonly IApiService _apiService;
    private readonly string _token;
    private readonly string _shopId;
    private readonly IServiceProvider _serviceProvider;

    private const string CreateOrderUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";

    private const string GetOrderUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail";

    private const string GetShippingFeeUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee";

    public GhnService(IApiService apiService, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _apiService = apiService;
        _serviceProvider = serviceProvider;
        _token = configuration["GHNExpress:Token"]!;
        _shopId = configuration["GHNExpress:ShopId"]!;
    }

    public async Task<ApiResponseModel> CreateShippingOrder(CreateGhnOrderModel model)
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
                Message = result.Error!.Message,
            };
        }

        var response = JsonSerializer.Deserialize<CreateGhnOrderResponseModel>(result.Value!);
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
            Data = response.Data.OrderCode
        };
    }


    public async Task<Result<GhnResponseModel>> GetShippingOrder(Guid customerId,
        string shippingCode)
    {
        Expression<Func<Order, bool>> predicate = x => x.ShippingCode == shippingCode;
        if (customerId != Guid.Empty)
        {
            Expression<Func<Order, bool>> customerExpression = x => x.CustomerId == customerId;
            predicate = CoreHelper.CombineAndAlsoExpressions(predicate, customerExpression);
        }

        var serviceProvider = _serviceProvider.CreateScope();
        var orderRepository = serviceProvider.ServiceProvider.GetRequiredService<IOrderRepository>();
        var order = await orderRepository.FindAll(predicate).FirstOrDefaultAsync();
        if (order == null)
        {
            return Result<GhnResponseModel>.Failure(DomainError.CommonError.NotFound(nameof(Order)));
        }

        var orderCode = new GetGhnOrderInfoModel
        {
            ShippingCode = shippingCode
        };
        var result = await _apiService.PostAsync(url: GetOrderUrl, data: orderCode,
            headers =>
            {
                headers.Add("Token", _token);
                headers.Add("User-Agent", "VStore");
            });
        if (!result.IsSuccess)
        {
            return Result<GhnResponseModel>.Failure(DomainError.ApiService.ApiCallFail);
        }

        var apiResponse = JsonSerializer.Deserialize<ApiResponseModel>(result.Value!);
        if (apiResponse == null)
        {
            return Result<GhnResponseModel>.Failure(new Error("400", "Invalid response"));
        }

        var response = JsonSerializer.Deserialize<GetGhnOrderInfoResponseModel>(apiResponse.Data!.ToString()!);
        if (response == null)
        {
            return Result<GhnResponseModel>.Failure(new Error("400", "Invalid response"));
        }

        return Result<GhnResponseModel>.Success(new GhnResponseModel
        {
            IsSuccess = true,
            Data = response
        });
    }

    public async Task<Result<GhnResponseModel>> GetShippingFee(Guid customerAddressId, Guid customerId)
    {
        var serviceProvider = _serviceProvider.CreateScope();
        var cartRepository = serviceProvider.ServiceProvider.GetRequiredService<ICartRepository>();
        var cart = await cartRepository.FindAll(x => x.CustomerId == customerId)
            .Include(x => x.CartDetails)
            .Include(x => x.Customer).ThenInclude(x => x.CustomerAddresses).FirstOrDefaultAsync();

        if (cart == null || cart.CartDetails.Count == 0)
        {
            return Result<GhnResponseModel>.Failure(DomainError.Cart.CartNotFoundOrEmpty);
        }

        var customerAddress = cart.Customer.CustomerAddresses.FirstOrDefault(x => x.Id == customerAddressId);
        if (customerAddress == null)
        {
            return Result<GhnResponseModel>.Failure(DomainError.CommonError.NotFound(nameof(CustomerAddress)));
        }

        var model = new GetGhnShippingFeeModel
        {
            ToWardCode = customerAddress.WardCode,
            ToDistrictId = customerAddress.DistrictId,
            Weight = cart.CartDetails.Sum(x => x.Product.Gram),
        };
        var result = await _apiService.PostAsync(url: GetShippingFeeUrl, data: model,
            headers =>
            {
                headers.Add("Token", _token);
                headers.Add("ShopId", _shopId);
            });
        if (!result.IsSuccess)
        {
            return Result<GhnResponseModel>.Failure(DomainError.ApiService.ApiCallFail);
        }

        var apiResponse = JsonSerializer.Deserialize<ApiResponseModel>(result.Value!);
        if (apiResponse == null)
        {
            return Result<GhnResponseModel>.Failure(new Error("400", "Invalid response"));
        }

        var response = JsonSerializer.Deserialize<GetGhnShippingFeeResponseModel>(apiResponse.Data!.ToString()!);
        if (response == null)
        {
            return Result<GhnResponseModel>.Failure(new Error("400", "Invalid response"));
        }

        return Result<GhnResponseModel>.Success(new GhnResponseModel
        {
            IsSuccess = true,
            Data = response
        });
    }
}
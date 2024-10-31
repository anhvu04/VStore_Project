using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VStore.Application.Abstractions.ApiService;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Models;
using VStore.Application.Usecases.GHNExpress.Common;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.GHNExpress.Query.GetProvince;

public record GetProvinceQueryHandler : IQueryHandler<GetProvinceQuery, List<GetProvinceModel>>
{
    private readonly IApiService _apiService;
    private const string Url = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province";
    private readonly string _token;

    public GetProvinceQueryHandler(IApiService apiService, IConfiguration configuration)
    {
        _apiService = apiService;
        _token = configuration["GHNExpress:Token"]!;
    }

    public async Task<Result<List<GetProvinceModel>>> Handle(GetProvinceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _apiService.GetAsync(Url, h => h.Add("Token", _token));
            if (!result.IsSuccess)
            {
                return Result<List<GetProvinceModel>>.Failure(DomainError.ApiService.ApiCallFail);
            }

            var response = JsonSerializer.Deserialize<ApiResponseModel>(result.Value!);
            if (response == null)
            {
                throw new Exception("Invalid response from GHN Express API.");
            }

            var data = JsonSerializer.Deserialize<List<GetProvinceModel>>(response.Data!.ToString()!);
            if (data == null)
            {
                throw new Exception("Invalid data from GHN Express API.");
            }

            return Result<List<GetProvinceModel>>.Success(data);
        }
        catch (Exception e)
        {
            return Result<List<GetProvinceModel>>.Failure(DomainError.ApiService.DeserializeFail(e.Message));
        }
    }
}
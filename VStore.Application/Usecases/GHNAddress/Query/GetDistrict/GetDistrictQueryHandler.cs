using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VStore.Application.Abstractions.ApiService;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Models;
using VStore.Application.Usecases.GHNAddress.Common;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.GHNAddress.Query.GetDistrict;

public class GetDistrictQueryHandler : IQueryHandler<GetDistrictQuery, List<GetDistrictModel>>
{
    private readonly IApiService _apiService;
    private readonly HttpClient _httpClient;
    private const string Url = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id=";
    private readonly string _token;

    public GetDistrictQueryHandler(IApiService apiService, HttpClient httpClient, IConfiguration configuration)
    {
        _apiService = apiService;
        _httpClient = httpClient;
        _token = configuration["GHNExpress:Token"]!;
    }

    public async Task<Result<List<GetDistrictModel>>> Handle(GetDistrictQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _apiService.GetAsync(Url + request.ProvinceId, h => h.Add("Token", _token));
            if (!result.IsSuccess)
            {
                return Result<List<GetDistrictModel>>.Failure(DomainError.ApiService.ApiCallFail);
            }

            var response = JsonSerializer.Deserialize<ApiResponseModel>(result.Value!);
            if (response == null)
            {
                throw new Exception("Invalid response from GHN Express API.");
            }

            var data = JsonSerializer.Deserialize<List<GetDistrictModel>>(response.Data!.ToString()!);
            if (data == null)
            {
                throw new Exception("Invalid data from GHN Express API.");
            }

            return Result<List<GetDistrictModel>>.Success(data);
        }
        catch (Exception e)
        {
            return Result<List<GetDistrictModel>>.Failure(DomainError.ApiService.DeserializeFail(e.Message));
        }
    }
}
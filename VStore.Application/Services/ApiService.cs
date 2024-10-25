using VStore.Application.Abstractions.ApiService;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<string>> GetAsync(string url, Action<Dictionary<string, string>> headers)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        Dictionary<string, string> header = new();
        headers(header);
        foreach (var item in header)
        {
            request.Headers.Add(item.Key, item.Value);
        }

        var response = await _httpClient.SendAsync(request);
        // get the response content
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return Result<string>.Failure(DomainError.ApiService.ApiCallFail);
        }

        return Result<string>.Success(content);
    }
}
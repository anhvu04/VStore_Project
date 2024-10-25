using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.ApiService;

public interface IApiService
{
    Task<Result<string>> GetAsync(string url, Action<Dictionary<string, string>> headers);
}
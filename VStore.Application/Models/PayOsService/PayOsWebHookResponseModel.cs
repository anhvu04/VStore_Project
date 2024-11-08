namespace VStore.Application.Models.PayOsService;

public record PayOsWebHookResponseModel
{
    public bool IsSuccess { get; set; }
    public object? Data { get; set; }

    public PayOsWebHookResponseModel(bool isSuccess, object? data)
    {
        IsSuccess = isSuccess;
        Data = data;
    }
}
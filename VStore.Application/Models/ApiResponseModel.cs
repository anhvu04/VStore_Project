using System.Text.Json.Serialization;

namespace VStore.Application.Models;

public class ApiResponseModel
{
    // define parameterless constructor to avoid error when deserializing
    public ApiResponseModel()
    {
    }

    public ApiResponseModel(int code, string? message, object? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public ApiResponseModel(int code, object? data)
    {
        Code = code;
        Data = data;
    }


    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("data")] public object? Data { get; set; }
}
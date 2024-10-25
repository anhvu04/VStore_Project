using System.Text.Json.Serialization;

namespace VStore.Application.Models;

public class ApiResponseModel
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("data")] public object? Data { get; set; }
}
using System.Text.Json.Serialization;

namespace VStore.Application.Models.PayOsService;

public record VerifyPayOsWebHookModel
{
    [JsonPropertyName("code")] public string Code { get; set; }
    [JsonPropertyName("desc")] public string Desc { get; set; }
    public bool Success { get; set; }
    [JsonPropertyName("data")] public object Data { get; set; }
    [JsonPropertyName("signature")] public string Signature { get; set; }
}
using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Order.Common;

namespace VStore.Application.Usecases.Order.Command.PayOsWebHook;

public record VerifyPayOsWebHookModel
{
    [JsonPropertyName("code")] public string Code { get; set; }
    [JsonPropertyName("desc")] public string Desc { get; set; }
    public bool Success { get; set; }
    [JsonPropertyName("data")] public object Data { get; set; }
    [JsonPropertyName("signature")] public string Signature { get; set; }
}
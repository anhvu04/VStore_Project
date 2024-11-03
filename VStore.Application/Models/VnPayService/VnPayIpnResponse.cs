using System.Text.Json.Serialization;

namespace VStore.Application.Models.VnPayService;

public class VnPayIpnResponse
{
    [JsonPropertyName("RspCode")] public string RspCode { get; set; }
    [JsonPropertyName("Message")] public string Message { get; set; }
}
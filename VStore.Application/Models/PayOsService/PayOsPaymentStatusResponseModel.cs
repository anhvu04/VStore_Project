using System.Text.Json.Serialization;

namespace VStore.Application.Models.PayOsService;

public record PayOsPaymentStatusResponseModel
{
    [JsonPropertyName("data")] public PayOsPaymentStatusDataModel? Data { get; set; }
}

public record PayOsPaymentStatusDataModel
{
    [JsonPropertyName("status")] public string Status { get; set; }
}
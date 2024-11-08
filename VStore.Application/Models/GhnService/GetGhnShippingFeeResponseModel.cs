using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class GetGhnShippingFeeResponseModel
{
    [JsonPropertyName("total")] public int Total { get; set; }
    [JsonPropertyName("service_fee")] public int ServiceFee { get; set; }
}
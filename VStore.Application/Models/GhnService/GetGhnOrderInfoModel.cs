using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class GetGhnOrderInfoModel
{
    [JsonPropertyName("order_code")] public string ShippingCode { get; set; }
}
using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class CreateGhnShippingOrderResponseModel
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("data")] public Data Data { get; set; }
}

public class Data
{
    [JsonPropertyName("order_code")] public string OrderCode { get; set; }
}
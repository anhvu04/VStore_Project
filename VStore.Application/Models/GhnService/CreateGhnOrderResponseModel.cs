using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class CreateGhnOrderResponseModel
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("data")] public CreateGhnResponseData Data { get; set; }
}

public class CreateGhnResponseData
{
    [JsonPropertyName("order_code")] public string OrderCode { get; set; }
}
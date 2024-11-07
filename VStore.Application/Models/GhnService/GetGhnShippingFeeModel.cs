using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class GetGhnShippingFeeModel
{
    [JsonPropertyName("to_ward_code")] public string ToWardCode { get; set; }
    [JsonPropertyName("to_district_id")] public int ToDistrictId { get; set; }
    [JsonPropertyName("weight")] public int Weight { get; set; }
    [JsonPropertyName("length")] public int Length { get; set; } = 50; // default 50cm
    [JsonPropertyName("width")] public int Width { get; set; } = 50; // default 50cm
    [JsonPropertyName("height")] public int Height { get; set; } = 50; // default 50cm
    [JsonPropertyName("service_type_id")] public int ServiceTypeId { get; set; } = 2; //2: E-commerce Delivery
}
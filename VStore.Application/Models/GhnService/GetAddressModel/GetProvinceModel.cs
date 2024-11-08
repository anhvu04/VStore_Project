using System.Text.Json.Serialization;

namespace VStore.Application.Usecases.GHNExpress.Common;

public record GetProvinceModel
{
    [JsonPropertyName("ProvinceID")] public int ProvinceId { get; set; }
    public string ProvinceName { get; set; }
}
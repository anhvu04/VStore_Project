using System.Text.Json.Serialization;

namespace VStore.Application.Usecases.GHNAddress.Common;

public record GetDistrictModel
{
    [JsonPropertyName("DistrictID")] public int DistrictId { get; set; }
    public string DistrictName { get; set; }
}
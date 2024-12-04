using System.Text.Json.Serialization;

namespace VStore.Application.Models.CloudinaryService;

public class CloudinaryResponseModel
{
    [JsonPropertyName("SecureUri")] public string SecureUri { get; set; }
    [JsonPropertyName("PublicId")] public string PublicId { get; set; }
}
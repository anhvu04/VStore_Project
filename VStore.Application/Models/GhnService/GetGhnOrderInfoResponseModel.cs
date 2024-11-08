using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class GetGhnOrderInfoResponseModel
{
    [JsonPropertyName("from_name")] public string FromName { get; set; }
    [JsonPropertyName("from_phone")] public string FromPhone { get; set; }
    [JsonPropertyName("to_name")] public string ToName { get; set; }
    [JsonPropertyName("to_phone")] public string ToPhone { get; set; }
    [JsonPropertyName("to_address")] public string ToAddress { get; set; }
    [JsonPropertyName("required_note")] public string RequiredNote { get; set; }
    [JsonPropertyName("note")] public string Note { get; set; }
    [JsonPropertyName("items")] public List<Items> Items { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("log")] public List<Log> Log { get; set; }
}

public class Log
{
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("updated_date")] public DateTime UpdatedDate { get; set; }
}
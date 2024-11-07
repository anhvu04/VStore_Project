using System.Text.Json.Serialization;

namespace VStore.Application.Models.GhnService;

public class CreateGhnShippingOrderModel
{
    [JsonPropertyName("to_name")] public string ToName { get; set; }
    [JsonPropertyName("to_phone")] public string ToPhone { get; set; }
    [JsonPropertyName("to_address")] public string ToAddress { get; set; }
    [JsonPropertyName("to_ward_code")] public string ToWardCode { get; set; }
    [JsonPropertyName("to_district_id")] public int ToDistrictId { get; set; }
    [JsonPropertyName("weight")] public int Weight { get; set; }
    [JsonPropertyName("length")] public int Length { get; set; } = 50; // default 50cm
    [JsonPropertyName("width")] public int Width { get; set; } = 50; // default 50cm
    [JsonPropertyName("height")] public int Height { get; set; } = 50; // default 50cm

    [JsonPropertyName("service_type_id")] public int ServiceTypeId { get; set; } = 2; //2: E-commerce Delivery
    [JsonPropertyName("payment_type_id")] public int PaymentTypeId { get; set; } // 1: Shop/Seller. 2: Buyer/Consignee.

    [JsonPropertyName("required_note")]
    public string RequiredNote { get; set; } = "CHOXEMHANGKHONGTHU"; //CHOTHUHANG, CHOXEMHANGKHONGTHU, KHONGCHOXEMHANG 

    [JsonPropertyName("note")] public string? Note { get; set; }

    [JsonPropertyName("pick_shift")]
    public List<int> PickShift { get; set; } = [4]; //"id":4 pick shift will be at tomorrow afternoon (12h00 - 18h00)",

    [JsonPropertyName("items")] public List<Items> Items { get; set; } = [];
}

public class Items
{
    [JsonPropertyName("name")] public string ProductName { get; set; }
    [JsonPropertyName("quantity")] public int Quantity { get; set; }
}
namespace VStore.Application.Usecases.CustomerAddress.Common;

public record CustomerAddressModel
{
    public string? ReceiverName { get; init; }
    public string? PhoneNumber { get; init; } = "";
    public string? Address { get; init; }
    public int ProvinceId { get; init; }
    public string? ProvinceName { get; init; }
    public int DistrictId { get; init; }
    public string? DistrictName { get; init; }
    public string? WardCode { get; init; }
    public string? WardName { get; init; }
    public bool IsDefault { get; set; } = false;
}
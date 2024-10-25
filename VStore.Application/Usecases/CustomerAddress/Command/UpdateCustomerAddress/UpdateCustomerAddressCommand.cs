using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;

namespace VStore.Application.Usecases.CustomerAddress.Command.UpdateCustomerAddress;

public record UpdateCustomerAddressCommand : CustomerAddressModel, ICommand
{
    [JsonIgnore] public Guid UserId { get; init; }
    [JsonIgnore] public new Guid Id { get; init; }
    public new string? PhoneNumber { get; init; }
    public new int? ProvinceId { get; init; }
    public new int? DistrictId { get; init; }
}
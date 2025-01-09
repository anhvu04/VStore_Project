using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Voucher.Common;

namespace VStore.Application.Usecases.Voucher.Command.UpdateVoucher;

public record UpdateVoucherCommand : VoucherModel, ICommand
{
    [JsonIgnore] public new Guid Id { get; set; }
}
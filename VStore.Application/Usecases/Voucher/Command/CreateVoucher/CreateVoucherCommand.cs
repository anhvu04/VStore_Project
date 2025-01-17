using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Voucher.Common;

namespace VStore.Application.Usecases.Voucher.Command.CreateVoucher;

public record CreateVoucherCommand : VoucherModel, ICommand
{
    [JsonIgnore] public new Guid Id { get; set; }
    public new required string Code { get; set; }
}
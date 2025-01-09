using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Voucher.Command.DeleteVoucher;

public record DeleteVoucherCommand(Guid Id) : ICommand
{
}
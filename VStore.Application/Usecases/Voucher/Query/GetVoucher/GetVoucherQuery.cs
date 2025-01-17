using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Voucher.Common;

namespace VStore.Application.Usecases.Voucher.Query.GetVoucher;

public record GetVoucherQuery(Guid Id) : IQuery<VoucherModel>

{
}
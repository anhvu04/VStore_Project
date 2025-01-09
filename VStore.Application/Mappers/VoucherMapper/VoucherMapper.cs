using AutoMapper;
using VStore.Application.Usecases.Voucher.Command.CreateVoucher;
using VStore.Application.Usecases.Voucher.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.VoucherMapper;

public class VoucherMapper : Profile
{
    public VoucherMapper()
    {
        CreateMap<CreateVoucherCommand, Voucher>();
        CreateMap<Voucher, VoucherModel>();
    }
}
using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Voucher.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Voucher.Query.GetVoucher;

public class GetVoucherQueryHandler : IQueryHandler<GetVoucherQuery, VoucherModel>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;

    public GetVoucherQueryHandler(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _mapper = mapper;
    }

    public async Task<Result<VoucherModel>> Handle(GetVoucherQuery request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.FindByIdAsync(request.Id, cancellationToken);
        if (voucher == null)
        {
            return Result<VoucherModel>.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Voucher)));
        }

        return Result<VoucherModel>.Success(_mapper.Map<VoucherModel>(voucher));
    }
}
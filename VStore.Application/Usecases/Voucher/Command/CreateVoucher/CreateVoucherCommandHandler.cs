using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Voucher.Command.CreateVoucher;

public class CreateVoucherCommandHandler : ICommandHandler<CreateVoucherCommand>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateVoucherCommandHandler(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        var code = await _voucherRepository.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (code)
        {
            return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Voucher.Code)));
        }

        var voucher = _mapper.Map<Domain.Entities.Voucher>(request);
        _voucherRepository.Add(voucher);
        await _unitOfWork.SaveChangesAsync(false,true,cancellationToken);
        return Result.Success();
    }
}
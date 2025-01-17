using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Voucher.Command.DeleteVoucher;

public class DeleteVoucherCommandHandler : ICommandHandler<DeleteVoucherCommand>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVoucherCommandHandler(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.FindByIdAsync(request.Id, cancellationToken);
        if (voucher == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Voucher)));
        }

        _voucherRepository.Remove(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
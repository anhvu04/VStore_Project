using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Voucher.Command.UpdateVoucher;

public class UpdateVoucherCommandHandler : ICommandHandler<UpdateVoucherCommand>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVoucherCommandHandler(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.FindByIdAsync(request.Id, cancellationToken);
        if (voucher == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Voucher)));
        }

        if (!string.IsNullOrEmpty(request.Code))
        {
            var code = await _voucherRepository.AnyAsync(x => x.Code == request.Code, cancellationToken);
            if (code)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Voucher.Code)));
            }

            voucher.Code = request.Code;
        }

        voucher.DiscountPercentage = request.DiscountPercentage ?? voucher.DiscountPercentage;
        voucher.MaxDiscountAmount = request.MaxDiscountAmount ?? voucher.MaxDiscountAmount;
        voucher.ExpiryDate = request.ExpiryDate ?? voucher.ExpiryDate;
        voucher.Quantity = request.Quantity ?? voucher.Quantity;
        voucher.IsActive = request.IsActive;
        _voucherRepository.Update(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
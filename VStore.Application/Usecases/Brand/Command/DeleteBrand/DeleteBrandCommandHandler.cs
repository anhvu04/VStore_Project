using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Brand.Command.DeleteBrand;

public class DeleteBrandCommandHandler : ICommandHandler<DeleteBrandCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandCommandHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.FindByIdAsync(request.Id, cancellationToken);
        if (brand == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Brand)));
        }

        var isExistProduct = await _brandRepository.IsBrandHasProductAsync(brand.Id, cancellationToken);
        if (isExistProduct)
        {
            return Result.Failure(DomainError.Brand.HasProduct);
        }

        brand.IsActive = false;
        _brandRepository.Remove(brand);
        await _unitOfWork.SaveChangesAsync(true, cancellationToken);
        return Result.Success();
    }
}
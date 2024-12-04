using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.ProductImage.Command.UpdateProductImage;

public class UpdateProductImageCommandHandler : ICommandHandler<UpdateProductImageCommand>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductImageCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork)
    {
        _productImageRepository = productImageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
    {
        var productImage = await _productImageRepository.FindByIdAsync(request.Id, cancellationToken);
        if (productImage == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.ProductImage)));
        }

        if (productImage.IsActive != request.IsActive)
        {
            productImage.IsActive = request.IsActive;
            _productImageRepository.Update(productImage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
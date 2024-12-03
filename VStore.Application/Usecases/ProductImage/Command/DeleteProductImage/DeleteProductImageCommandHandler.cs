using VStore.Application.Abstractions.CloudinaryService;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.ProductImage.Command.DeleteProductImage;

public class DeleteProductImageCommandHandler : ICommandHandler<DeleteProductImageCommand>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public DeleteProductImageCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork,
        ICloudinaryService cloudinaryService)
    {
        _productImageRepository = productImageRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<Result> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var productImage = await _productImageRepository.FindByIdAsync(request.Id, cancellationToken);
        if (productImage == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.ProductImage)));
        }

        await _cloudinaryService.DeleteImageAsync(productImage.PublicId);
        _productImageRepository.Remove(productImage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
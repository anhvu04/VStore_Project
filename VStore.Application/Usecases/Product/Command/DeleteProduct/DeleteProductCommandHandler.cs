using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Product.Command.DeleteProduct;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Product)));
        }

        product.IsActive = false;
        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(true, cancellationToken);
        return Result.Success();
    }
}
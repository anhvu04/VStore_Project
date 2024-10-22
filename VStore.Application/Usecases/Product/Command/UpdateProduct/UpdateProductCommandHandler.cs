using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Product.Command.UpdateProduct;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IProductRepository productRepository,
        IBrandRepository brandRepository, ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Product)));
        }

        product.OriginalPrice = request.OriginalPrice ?? product.OriginalPrice;
        product.SalePrice = request.SalePrice ?? product.SalePrice;
        if (product.SalePrice > product.OriginalPrice)
        {
            return Result.Failure(DomainError.Product.InvalidPrice);
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            var isExistProductName = await _productRepository.AnyAsync(
                x => x.Name == request.Name && x.Id != request.Id,
                cancellationToken);
            if (isExistProductName)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Product.Name)));
            }

            product.Name = request.Name;
        }

        if (request.BrandId.HasValue)
        {
            var isExistBrand = await _brandRepository.AnyAsync(x => x.Id == request.BrandId, cancellationToken);
            if (!isExistBrand)
            {
                return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Brand)));
            }

            product.BrandId = request.BrandId.Value;
        }

        if (request.CategoryId.HasValue)
        {
            var isExistCategory =
                await _categoryRepository.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);
            if (!isExistCategory)
            {
                return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category)));
            }

            product.CategoryId = request.CategoryId.Value;
        }

        product.Description = request.Description ?? product.Description;
        product.Quantity = request.Quantity ?? product.Quantity;
        product.Gram = request.Gram ?? product.Gram;
        product.Status = request.Status.HasValue ? (ProductStatus)request.Status.Value : product.Status;
        product.Thumbnail = request.Thumbnail ?? product.Thumbnail;
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}
using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Product.Command.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, IBrandRepository brandRepository,
        ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var isExistProductName = await _productRepository.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (isExistProductName)
        {
            return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Product.Name)));
        }

        var isExistBrand = await _brandRepository.AnyAsync(x => x.Id == request.BrandId, cancellationToken);
        if (!isExistBrand)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Brand)));
        }

        var isExistCategory = await _categoryRepository.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);
        if (!isExistCategory)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category)));
        }

        var product = _mapper.Map<Domain.Entities.Product>(request);
        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}
using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Product.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Product.Query.GetProduct;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductResponseModel>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<ProductResponseModel>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.FindByIdAsync(request.Id, cancellationToken, x => x.Category, x => x.Brand);
        if (product is null)
        {
            return Result<ProductResponseModel>.Failure(
                DomainError.CommonError.NotFound(nameof(Domain.Entities.Product)));
        }

        return Result<ProductResponseModel>.Success(_mapper.Map<ProductResponseModel>(product));
    }
}
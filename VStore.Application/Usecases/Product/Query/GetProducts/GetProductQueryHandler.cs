using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Product.Common;
using VStore.Application.Usecases.Product.Query.GetProduct;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Product.Query.GetProducts;

public class GetProductQueryHandler : IQueryHandler<GetProductsQuery, PageList<ProductResponseModel>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<PageList<ProductResponseModel>>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Product, bool>> filter = x => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Domain.Entities.Product, bool>> searchFilter = x =>
                x.Name.Contains(request.SearchTerm) || x.Description!.Contains(request.SearchTerm);
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, searchFilter);
        }

        if (request.MinPrice.HasValue)
        {
            Expression<Func<Domain.Entities.Product, bool>> minPriceFilter = x =>
                x.SalePrice == 0 ? x.OriginalPrice >= request.MinPrice : x.SalePrice >= request.MinPrice;
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, minPriceFilter);
        }

        if (request.MaxPrice.HasValue)
        {
            Expression<Func<Domain.Entities.Product, bool>> maxPriceFilter = x =>
                x.SalePrice == 0 ? x.OriginalPrice <= request.MaxPrice : x.SalePrice <= request.MaxPrice;
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, maxPriceFilter);
        }

        if (request.IsActive.HasValue)
        {
            Expression<Func<Domain.Entities.Product, bool>> activeFilter = x => x.IsActive == request.IsActive;
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, activeFilter);
        }

        if (request.CategoryId.Length > 0 || request.BrandId.Length > 0 || request.StatusId.Length > 0)
        {
            Expression<Func<Domain.Entities.Product, bool>> idFilter = x =>
                (request.CategoryId.Contains(x.CategoryId) ||
                 request.BrandId.Contains(x.BrandId) ||
                 request.StatusId.Contains((int)x.Status)) && x.Category.IsActive && x.Brand.IsActive;
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, idFilter);
        }

        var query = _productRepository.FindAll(filter);
        var sortProperty = GetSortProperty(request.SortBy);
        query = query.ApplySorting(request.IsDescending, sortProperty);
        var queryModel = query.ProjectTo<ProductResponseModel>(_mapper.ConfigurationProvider);
        return await PageList<ProductResponseModel>.CreateAsync(queryModel, request.Page, request.PageSize);
    }

    private Expression<Func<Domain.Entities.Product, object>> GetSortProperty(string? requestSortBy) =>
        requestSortBy?.ToLower().Replace(" ", "") switch
        {
            "name" => product => product.Name,
            "price" => product => product.SalePrice == 0 ? product.OriginalPrice : product.SalePrice,
            "createddate" => product => product.CreatedDate,
            _ => product => product.Id
        };
}
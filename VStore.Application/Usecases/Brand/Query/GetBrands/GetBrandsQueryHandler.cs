using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Brand.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Brand.Query.GetBrands;

public class GetBrandsQueryHandler : IQueryHandler<GetBrandsQuery, PageList<BrandModel>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public GetBrandsQueryHandler(IBrandRepository brandRepository, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _mapper = mapper;
    }

    public async Task<Result<PageList<BrandModel>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Brand, bool>> filter = brand => true;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            Expression<Func<Domain.Entities.Brand, bool>> searchFilter = brand =>
                brand.Name.Contains(request.SearchTerm) || brand.Description!.Contains(request.SearchTerm);
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, searchFilter);
        }

        if (request.IsActive.HasValue)
        {
            Expression<Func<Domain.Entities.Brand, bool>> activeFilter = brand => brand.IsActive == request.IsActive;
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, activeFilter);
        }

        var query = _brandRepository.FindAll(filter);
        var sortProperty = GetSortProperty(request.SortBy);
        query = query.ApplySorting(request.IsDescending, sortProperty);
        var queryModel = query.ProjectTo<BrandModel>(_mapper.ConfigurationProvider);
        return await PageList<BrandModel>.CreateAsync(queryModel, request.Page, request.PageSize);
    }

    private Expression<Func<Domain.Entities.Brand, object>> GetSortProperty(string? requestSortBy) =>
        requestSortBy?.ToLower().Replace(" ", "") switch
        {
            "name" => brand => brand.Name,
            _ => brand => brand.Id
        };
}
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Category.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Category.Query.GetCategories;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, PageList<CategoryModel>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<PageList<CategoryModel>>> Handle(GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Category, bool>> filter = category => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Domain.Entities.Category, bool>> searchFilter = category =>
                category.Name.Contains(request.SearchTerm) || category.Description!.Contains(request.SearchTerm);
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, searchFilter);
        }

        if (request.IsActive.HasValue)
        {
            Expression<Func<Domain.Entities.Category, bool>> activeFilter = category =>
                category.IsActive == request.IsActive;
            filter = CoreHelper.CoreHelper.CombineAndAlsoExpressions(filter, activeFilter);
        }

        var category = _categoryRepository.FindAll(filter);
        var sortProperty = GetSortProperty(request.SortBy);
        category = category.ApplySorting(request.IsDescending, sortProperty);
        var categoryModel = category.ProjectTo<CategoryModel>(_mapper.ConfigurationProvider);
        return await PageList<CategoryModel>.CreateAsync(categoryModel, request.Page, request.PageSize);
    }

    private Expression<Func<Domain.Entities.Category, object>> GetSortProperty(string? requestSortBy) =>
        requestSortBy?.ToLower().Replace(" ", "") switch
        {
            "name" => category => category.Name,
            _ => category => category.Id
        };
}
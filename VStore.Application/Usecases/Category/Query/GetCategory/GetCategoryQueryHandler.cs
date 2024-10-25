using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Category.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Category.Query.GetCategory;

public class GetCategoryQueryHandler : IQueryHandler<GetCategoryQuery, CategoryModel>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<CategoryModel>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindAll(x => x.Id == request.Id)
            .ProjectTo<CategoryModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        if (category == null)
        {
            return Result<CategoryModel>.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category)));
        }

        return Result<CategoryModel>.Success(category);
    }
}
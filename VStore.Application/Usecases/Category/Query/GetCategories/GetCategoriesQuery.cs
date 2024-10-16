using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.QueryModel;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Category.Common;

namespace VStore.Application.Usecases.Category.Query.GetCategories;

public class GetCategoriesQuery : PageModel, ISearchModel, ISortModel, IQuery<PageList<CategoryModel>>
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public bool? IsActive { get; set; }
}
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.QueryModel;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Brand.Common;

namespace VStore.Application.Usecases.Brand.Query.GetBrands;

public class GetBrandsQuery : PageModel, ISearchModel, ISortModel, IQuery<PageList<BrandModel>>
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public bool? IsActive { get; set; }
}
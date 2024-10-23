using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.QueryModel;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Product.Common;

namespace VStore.Application.Usecases.Product.Query.GetProducts;

public class GetProductsQuery : PageModel, ISearchModel, ISortModel, IQuery<PageList<ProductResponseModel>>
{
    public string? SearchTerm { get; set; }
    public int[] CategoryId { get; set; } = [];
    public int[] BrandId { get; set; } = [];
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }

    public int[] StatusId { get; set; } = [];
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
}
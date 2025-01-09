using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.QueryModel;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Voucher.Common;

namespace VStore.Application.Usecases.Voucher.Query.GetVouchers;

public class GetVouchersQuery : PageModel, ISearchModel, ISortModel, IQuery<PageList<VoucherModel>>
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public bool? IsActive { get; set; }
}
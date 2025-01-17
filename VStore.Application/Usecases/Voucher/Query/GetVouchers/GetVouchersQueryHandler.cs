using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Voucher.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Voucher.Query.GetVouchers;

public class GetVouchersQueryHandler : IQueryHandler<GetVouchersQuery, PageList<VoucherModel>>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;

    public GetVouchersQueryHandler(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _mapper = mapper;
    }

    public async Task<Result<PageList<VoucherModel>>> Handle(GetVouchersQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Voucher, bool>> predicate = x => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Domain.Entities.Voucher, bool>> filter = x =>
                x.Code.Contains(request.SearchTerm);
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, filter);
        }

        if (request.IsActive.HasValue)
        {
            Expression<Func<Domain.Entities.Voucher, bool>> filter = x =>
                x.IsActive == request.IsActive;
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, filter);
        }

        var query = _voucherRepository.FindAll(predicate);
        var sortProperty = GetSortProperty(request.SortBy);
        query = query.ApplySorting(request.IsDescending, sortProperty);
        var queryModel = query.ProjectTo<VoucherModel>(_mapper.ConfigurationProvider);
        return await PageList<VoucherModel>.CreateAsync(queryModel, request.Page, request.PageSize);
    }

    private Expression<Func<Domain.Entities.Voucher, object>> GetSortProperty(string? requestSortBy) =>
        requestSortBy?.ToLower().Replace(" ", "") switch
        {
            "quantity" => x => x.Quantity,
            "discountPercentage" => x => x.DiscountPercentage,
            "maxDiscountAmount" => x => x.MaxDiscountAmount,
            _ => x => x.ExpiryDate
        };
}
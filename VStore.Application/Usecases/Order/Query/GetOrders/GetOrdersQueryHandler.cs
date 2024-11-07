using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Order.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Query.GetOrders;

public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, PageList<OrderModel>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IMapper mapper, IOrderRepository orderRepository)
    {
        _mapper = mapper;
        _orderRepository = orderRepository;
    }

    public async Task<Result<PageList<OrderModel>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Order, bool>> predicate = x => true;
        if (request.CustomerId != Guid.Empty)
        {
            Expression<Func<Domain.Entities.Order, bool>> userPredicate = x => x.CustomerId == request.CustomerId;
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, userPredicate);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Domain.Entities.Order, bool>> searchPredicate =
                x => x.OrderDetails.Any(y => y.ProductName.Contains(request.SearchTerm));
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, searchPredicate);
        }

        if (request.OrderStatus.HasValue)
        {
            Expression<Func<Domain.Entities.Order, bool>> statusPredicate =
                x => (int)x.Status == request.OrderStatus;
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, statusPredicate);
        }

        if (request.PaymentMethod.HasValue)
        {
            Expression<Func<Domain.Entities.Order, bool>> paymentPredicate =
                x => (int)x.PaymentMethod == request.PaymentMethod;
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, paymentPredicate);
        }

        var query = _orderRepository.FindAll(predicate);
        query = request.CustomerId == Guid.Empty
            ? query.OrderBy(x => x.Status).ThenByDescending(x => x.CreatedDate)
            : query.OrderByDescending(x => x.CreatedDate).ThenBy(x => x.Status);
        var queryModel = query.ProjectTo<OrderModel>(_mapper.ConfigurationProvider);
        return await PageList<OrderModel>.CreateAsync(queryModel, request.Page, request.PageSize);
    }
}
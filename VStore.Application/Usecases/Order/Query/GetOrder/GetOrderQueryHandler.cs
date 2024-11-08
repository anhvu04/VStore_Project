using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Order.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Query.GetOrder;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDetailModel>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IMapper mapper, IOrderRepository orderRepository)
    {
        _mapper = mapper;
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDetailModel>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Order, bool>> predicate = x => x.Id == request.OrderId;
        if (request.CustomerId != Guid.Empty)
        {
            Expression<Func<Domain.Entities.Order, bool>> customerPredicate = x => x.CustomerId == request.CustomerId;
            predicate = CoreHelper.CoreHelper.CombineAndAlsoExpressions(predicate, customerPredicate);
        }

        var order = await _orderRepository.FindAll(predicate).Include(x => x.OrderDetails)
            .ProjectTo<OrderDetailModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        if (order == null)
        {
            return Result<OrderDetailModel>.Failure(DomainError.CommonError.NotFound(nameof(Order)));
        }

        return Result<OrderDetailModel>.Success(order);
    }
}
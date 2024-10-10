using MediatR;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.MediatR;

public interface IQueryHandler<in TQuery> : IRequestHandler<TQuery, Result> where TQuery : IQuery
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
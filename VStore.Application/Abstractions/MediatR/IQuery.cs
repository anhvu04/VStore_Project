using MediatR;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.MediatR;

public interface IQuery : IRequest<Result>
{
}

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
using MediatR;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.MediatR;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
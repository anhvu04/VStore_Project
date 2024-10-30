using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Command.PayOsWebHook;

public class PayOsWebHookCommandHandler : ICommandHandler<PayOsWebHookCommand>
{
    public async Task<Result> Handle(PayOsWebHookCommand request, CancellationToken cancellationToken)
    {
        return Result.Success();
    }
}
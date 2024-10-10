using FluentValidation;
using MediatR;
using VStore.Domain.Shared;

namespace VStore.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<Result>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
        {
            return await next();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            .ToArray();

        return (TResponse)(object)Result.Failure(new ValidationError(errors));
    }
}
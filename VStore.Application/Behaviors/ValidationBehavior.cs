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

        // Check if TResponse is a generic type Result<T>
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArgument = typeof(TResponse).GetGenericArguments()[0];
            var failureResult = CreateFailureResult(genericArgument, errors);
            return (TResponse)failureResult;
        }

        if (typeof(TResponse) == typeof(Result))
        {
            // Handle non-generic Result type
            return (TResponse)(object)Result.Failure(new ValidationError(errors));
        }

        // Handle unexpected types gracefully
        throw new InvalidOperationException("Invalid response type.");
    }

    private static object CreateFailureResult(Type genericArgument, Error[] errors)
    {
        var resultType = typeof(Result<>).MakeGenericType(genericArgument);
        var failureResult = Activator.CreateInstance(resultType, null, false, new ValidationError(errors));
        return failureResult!;
    }
}
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VStore.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        //log detailed exception information
        _logger.LogError(
            "An error occurred while processing your request: {ExceptionName}\nMessage: {Message}\nStack Trace: {StackTrace}",
            exception.GetType().Name, exception.Message, exception.StackTrace);

        if (exception.InnerException != null)
        {
            _logger.LogError("Inner Exception: {InnerMessage}\nInner Stack Trace: {InnerStackTrace}",
                exception.InnerException.Message, exception.InnerException.StackTrace);
        }

        httpContext.Response.StatusCode = GetStatusCode(exception);
        //prevent showing the actual exception message to the client
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Status = httpContext.Response.StatusCode,
            Title = "An error occurred while processing your request",
            // this detail is only for debugging purposes
            // Detail = $"{exception.GetType().Name}: {exception.Message}" +
            //          $"\nInner Exception: {exception.InnerException?.Message}",
            Type = GetHelpLink(exception)
        };
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken: cancellationToken
        );
        return true;
    }

    /// <summary>
    /// Get help link based on the exception type
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    private static string GetHelpLink(Exception exception)
    {
        return exception switch
        {
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6"
        };
    }

    /// <summary>
    /// Get status code based on the exception type
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
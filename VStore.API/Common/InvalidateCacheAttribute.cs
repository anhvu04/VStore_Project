using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VStore.Application.Abstractions.RedisCartService;

namespace VStore.API.Common;

[AttributeUsage(AttributeTargets.All)]
public class InvalidateCacheAttribute(string pattern) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (resultContext.Exception == null || resultContext.ExceptionHandled)
        {
            if (resultContext.Result is OkObjectResult { Value: not null })
            {
                var requiredService = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheResponse>();
                await requiredService.DeleteCacheResponseAsync(pattern);
            }
        }
    }
}
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using VStore.Application.Abstractions.RedisCartService;

namespace VStore.API.Common;

[AttributeUsage(AttributeTargets.All)]
public class CacheResponseAttribute(int timeToLiveInSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var requiredService = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheResponse>();
        var key = GetCacheKey(context.HttpContext.Request);
        var cacheResponse = await requiredService.GetCacheResponseAsync(key);
        if (!string.IsNullOrEmpty(cacheResponse))
        {
            context.Result = new ContentResult
            {
                Content = cacheResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            return;
        }

        var executedContext = await next();
        if (executedContext.Result is ObjectResult objectResult)
        {
            if (objectResult.Value != null)
            {
                await requiredService.SetCacheResponseAsync(key, objectResult.Value,
                    TimeSpan.FromMinutes(timeToLiveInSeconds));
            }
        }
    }

    private string GetCacheKey(HttpRequest request)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"{request.Path}/");
        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            stringBuilder.Append($"{key}-{value}");
        }

        return stringBuilder.ToString();
    }
}
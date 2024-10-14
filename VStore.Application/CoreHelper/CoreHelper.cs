using Microsoft.AspNetCore.Http;

namespace VStore.Application.CoreHelper;

public class CoreHelper
{
    public static string GetTokenFromContext(HttpContext context)
    {
        return context.Request.Headers.Authorization.ToString().Split(" ")[1];
    }
}
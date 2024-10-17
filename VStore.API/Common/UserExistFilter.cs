using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VStore.Domain.Abstractions.Repositories;

namespace VStore.API.Common;

public class UserExistFilter : IAsyncActionFilter
{
    private readonly IUserRepository _userRepository;

    public UserExistFilter(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = Guid.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ??
                                Guid.Empty.ToString());
        var existUser = await _userRepository.AnyAsync(x => x.Id == userId);
        if (!existUser)
        {
            context.Result = new NotFoundObjectResult("User not found");
            return;
        }

        // context.HttpContext.Items.Add("UserId", userId);
        // If the user exists, continue with the next action
        await next();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

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
        var existUser = await _userRepository.FindByIdAsync(userId);
        if (existUser == null)
        {
            var result = new Result(false, DomainError.CommonError.NotFound(nameof(User)));
            context.Result = new BadRequestObjectResult(result.Error);
            return;
        }

        if (existUser.IsBanned)
        {
            var result = new Result(false, DomainError.User.Banned);
            context.Result = new BadRequestObjectResult(result.Error);
            return;
        }


        // context.HttpContext.Items.Add("UserId", userId);
        // If the user exists, continue with the next action
        await next();
    }
}
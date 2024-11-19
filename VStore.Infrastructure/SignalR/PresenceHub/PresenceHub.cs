using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VStore.Domain.AuthenticationScheme;

namespace VStore.Infrastructure.SignalR.PresenceHub;

[Authorize(AuthenticationSchemes = AuthenticationScheme.Access)]
public class PresenceHub : Hub
{
    private readonly PresenceTracker _presenceTracker;

    public PresenceHub(PresenceTracker presenceTracker)
    {
        _presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Guid.Parse(Context.User!.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty);
        await _presenceTracker.OnConnectedAsync(userId, Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", userId + " is online");
        await Clients.All.SendAsync("GetOnlineUsers", await _presenceTracker.GetOnlineUsers());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Guid.Parse(Context.User!.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty);
        await _presenceTracker.OnDisconnectedAsync(userId, Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", userId + " is offline");
        await Clients.All.SendAsync("GetOnlineUsers", await _presenceTracker.GetOnlineUsers());
        await base.OnDisconnectedAsync(exception);
    }
}
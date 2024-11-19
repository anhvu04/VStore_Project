namespace VStore.Infrastructure.SignalR.PresenceHub;

public class PresenceTracker
{
    // Dictionary to store the online users and their connections (connectionId).
    // Each user can have multiple connections due to multiple tabs or devices
    private readonly Dictionary<Guid, List<string>> _onlineUsers = new();

    /// <summary>
    /// Add the connectionId to the list of connections for the user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public Task OnConnectedAsync(Guid userId, string connectionId)
    {
        lock (_onlineUsers)
        {
            if (_onlineUsers.TryGetValue(userId, out var connections))
            {
                connections.Add(connectionId);
            }
            else
            {
                _onlineUsers[userId] = new List<string> { connectionId };
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove the connectionId from the list of connections for the user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public Task OnDisconnectedAsync(Guid userId, string connectionId)
    {
        lock (_onlineUsers)
        {
            if (!_onlineUsers.TryGetValue(userId, out var connections))
            {
                return Task.CompletedTask;
            }

            connections.Remove(connectionId);

            // Remove the user from the OnlineUsers dictionary if they have no connections
            // (no connections means they are offline with no tabs open or devices connected)
            if (connections.Count == 0)
            {
                _onlineUsers.Remove(userId);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Get all the online users for the presence hub.
    /// </summary>
    /// <returns></returns>
    public Task<Guid[]> GetOnlineUsers()
    {
        Guid[] onlineUserIds;
        lock (_onlineUsers)
        {
            onlineUserIds = _onlineUsers.Keys.ToArray();
        }

        return Task.FromResult(onlineUserIds);
    }

    /// <summary>
    /// Get all user connectionsIds by userId for sending notifications when a user is online.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<string>?> GetConnectionsForUser(Guid userId)
    {
        List<string>? connections;
        lock (_onlineUsers)
        {
            connections = _onlineUsers.GetValueOrDefault(userId);
        }

        return Task.FromResult(connections);
    }
}
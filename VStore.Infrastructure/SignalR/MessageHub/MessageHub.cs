using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VStore.Application.Models.SignalRService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Infrastructure.SignalR.PresenceHub;

namespace VStore.Infrastructure.SignalR.MessageHub;

[Authorize(AuthenticationSchemes = AuthenticationScheme.Access)]
public class MessageHub : Hub
{
    private readonly ILogger<MessageHub> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PresenceTracker _presenceTracker;
    private readonly IHubContext<PresenceHub.PresenceHub> _presenceHubContext;

    public MessageHub(ILogger<MessageHub> logger, IUserRepository userRepository, IMessageRepository messageRepository,
        IGroupRepository groupRepository, IUnitOfWork unitOfWork, IMapper mapper, PresenceTracker presenceTracker,
        IHubContext<PresenceHub.PresenceHub> presenceHubContext)
    {
        _logger = logger;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _presenceTracker = presenceTracker;
        _presenceHubContext = presenceHubContext;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogInformation("Connected to message hub");
            var currentUserId =
                Guid.Parse(Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty);
            var otherUserId =
                Guid.Parse(Context.GetHttpContext()?.Request.Query["otherUser"].ToString() ?? string.Empty);

            // Validate the user and the other user when connecting to the message hub
            await ValidateUser(currentUserId, otherUserId);

            // Check if the user is already connected
            var isExistConnection = await _groupRepository.AnyAsync(
                x => x.Connections.Any(c => c.UserId == currentUserId));
            if (isExistConnection)
            {
                _logger.LogInformation("User {currentUserId} is already connected", currentUserId);
                throw new HubException($"User {currentUserId} is already connected");
            }

            await JoinGroup(currentUserId, otherUserId);
            await ReceiveMessage(currentUserId, otherUserId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error connecting to message hub");
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Disconnected from message hub");
        await LeaveGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Guid senderId, Guid receiverId, string message)
    {
        try
        {
            var groupName = GetGroupName(senderId, receiverId);
            var group = await _groupRepository
                .FindAll(
                    x => x.Connections.Any(c =>
                        c.UserId == senderId && c.Id == Context.ConnectionId && c.GroupName == groupName),
                    x => x.Connections)
                .FirstOrDefaultAsync();
            if (group == null)
            {
                _logger.LogError("User {senderId} is not in group", senderId);
                throw new HubException($"User {senderId} is not in group");
            }

            var newMessage = new Message
            {
                SenderId = senderId,
                RecipientId = receiverId,
                Content = message
            };

            var isExistRecipient = group.Connections.Any(x => x.UserId == receiverId);
            if (isExistRecipient)
            {
                newMessage.IsRead = true;
                newMessage.DateRead = DateTime.UtcNow;
            }
            else
            {
                // Notify the recipient that they have a new message if they are online but not in the group
                var connectionIds = await _presenceTracker.GetConnectionsForUser(receiverId);
                if (connectionIds != null)
                {
                    await _presenceHubContext.Clients.Clients(connectionIds).SendAsync("NewMessageReceived",
                        "You have a new message from " + senderId);
                }
            }

            _messageRepository.Add(newMessage);
            var messageModels = _mapper.Map<MessageModel>(newMessage);
            await _unitOfWork.SaveChangesAsync(false, true);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", messageModels);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending message");
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }

    private async Task ReceiveMessage(Guid senderId, Guid receiverId)
    {
        try
        {
            var messages = await _messageRepository
                .FindAll(x => x.SenderId == senderId && x.RecipientId == receiverId ||
                              x.SenderId == receiverId && x.RecipientId == senderId && x.RecipientDeleted == false)
                .OrderBy(x => x.CreatedDate).ToListAsync();
            var unreadMessages = messages.Where(x => x.SenderId == receiverId && x.IsRead == false).ToList();
            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.DateRead = DateTime.UtcNow;
            }

            _messageRepository.UpdateRange(unreadMessages);
            await _unitOfWork.SaveChangesAsync();
            var messageModels = _mapper.Map<List<MessageModel>>(messages);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messageModels);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error receiving message");
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }

    private async Task JoinGroup(Guid senderId, Guid receiverId)
    {
        try
        {
            var groupName = GetGroupName(senderId, receiverId);
            var isExistGroup =
                await _groupRepository.FindByIdAsync(groupName, cancellationToken: default, x => x.Connections);
            if (isExistGroup == null)
            {
                isExistGroup = new Group
                {
                    Id = groupName
                };
                _groupRepository.Add(isExistGroup);
            }

            var isExistConnection = isExistGroup.Connections.Any(x => x.UserId == senderId);
            if (isExistConnection)
            {
                _logger.LogInformation("User {senderId} is already in group", senderId);
                throw new HubException($"User {senderId} is already in group");
            }

            isExistGroup.Connections.Add(new Connection
            {
                Id = Context.ConnectionId,
                UserId = senderId
            });
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Join group {groupName}", groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", "User: " + senderId + " has joined the group");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error joining group");
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }

    private async Task LeaveGroup()
    {
        try
        {
            var group = await _groupRepository
                .FindAll(x => x.Connections.Any(c => c.Id == Context.ConnectionId),
                    x => x.Connections)
                .FirstOrDefaultAsync();
            if (group == null)
            {
                _logger.LogInformation("Group not found");
                throw new HubException("Group not found");
            }

            var isExistConnection = group.Connections.FirstOrDefault(x => x.Id == Context.ConnectionId);
            if (isExistConnection == null)
            {
                _logger.LogInformation("User is not in group");
                throw new HubException("User is not in group");
            }

            _groupRepository.RemoveConnection(isExistConnection);
            await _unitOfWork.SaveChangesAsync();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id);
            await Clients.Group(group.Id).SendAsync("UpdatedGroup", "User: " + isExistConnection.UserId +
                                                                    " has left the group");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error leaving group");
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }

    private string GetGroupName(Guid senderId, Guid receiverId)
    {
        var compare = String.CompareOrdinal(senderId.ToString(), receiverId.ToString());
        return compare < 0 ? $"{senderId}-{receiverId}" : $"{receiverId}-{senderId}";
    }

    private async Task ValidateUser(Guid currentUserId, Guid otherUserId)
    {
        if (currentUserId == Guid.Empty || otherUserId == Guid.Empty)
        {
            _logger.LogError("Invalid user id {currentUserId} or {otherUserId}", currentUserId, otherUserId);
            throw new HubException($"Invalid user id {currentUserId} or {otherUserId}");
        }

        var currentUser = await _userRepository.FindByIdAsync(currentUserId);
        if (currentUser == null || currentUser.IsBanned)
        {
            _logger.LogError("User ${currentUserId} not found or is banned", currentUserId);
            throw new HubException($"User ${currentUserId} not found or is banned");
        }

        var currentUserRole = currentUser.Role.ToString();

        var otherUser = await _userRepository.FindAll(x => x.Id == otherUserId)
            .FirstOrDefaultAsync();
        if (otherUser == null || otherUser.IsBanned)
        {
            _logger.LogError("User ${otherUserId} not found or is banned", otherUserId);
            throw new HubException($"User ${otherUserId} not found or is banned");
        }

        // Check if the user is a customer and the other user is an admin or vice versa
        if (currentUserRole == Role.Customer.ToString() && otherUser.Role != Role.Admin ||
            currentUserRole == Role.Admin.ToString() && otherUser.Role != Role.Customer)
        {
            _logger.LogError("Invalid chat attempt between {currentUserId} and {otherUserId}", currentUserId,
                otherUserId);
            throw new HubException($"Invalid chat attempt between {currentUserId} and {otherUserId}");
        }
    }
}
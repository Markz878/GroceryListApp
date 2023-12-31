using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GroceryListHelper.Server.Hubs;

[Authorize]
public sealed class CartHub(IMediator mediator) : Hub<ICartHubNotifications>, ICartHubClient
{
    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public async Task JoinGroup(Guid groupId)
    {
        await CheckAccess(groupId);
        await Groups.AddToGroupAsync(GetConnectionId(), groupId.ToString());
        await Clients.OthersInGroup(groupId.ToString()).GetMessage($"{GetUserEmail()} has joined cart sharing.");
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await CheckAccess(groupId);
        await Groups.RemoveFromGroupAsync(GetConnectionId(), groupId.ToString());
        await Clients.OthersInGroup(groupId.ToString()).GetMessage($"{GetUserEmail()} has left cart sharing.");
    }

    private async Task CheckAccess(Guid groupId)
    {
        if (await mediator.Send(new CheckGroupAccessQuery() { GroupId = groupId, UserEmail = GetUserEmail() }) is false)
        {
            throw new UnauthorizedAccessException("User is not part of the group");
        }
    }

    private string GetUserEmail()
    {
        string? email = Context.User?.FindFirst(AuthenticationConstants.EmailClaimName)?.Value;
        ArgumentNullException.ThrowIfNull(email);
        return email;
    }
}

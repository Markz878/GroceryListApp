namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubClient
{
    Task JoinGroup(Guid groupId);
    Task LeaveGroup(Guid groupId);
    string? GetConnectionId();
}
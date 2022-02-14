namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubActions
{
    Task<HubResponse> CreateGroup(List<string> allowedUsers);
    Task<HubResponse> JoinGroup(string hostEmail);
    Task<HubResponse> LeaveGroup();
    Task<string> CartItemAdded(CartProductCollectable product);
    Task CartItemModified(CartProductCollectable product);
    Task CartItemDeleted(string id);
}

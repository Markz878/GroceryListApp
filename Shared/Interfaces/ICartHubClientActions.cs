using GroceryListHelper.Shared.Models.BaseModels;
using GroceryListHelper.Shared.Models.CartProducts;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubClientActions
{
    Task<HubResponse> JoinGroup(Guid groupId);
    Task<HubResponse> LeaveGroup(Guid groupId);
    Task<HubResponse> CartItemAdded(CartProduct product);
    Task<HubResponse> CartItemModified(CartProductCollectable product);
    Task<HubResponse> CartItemDeleted(string name);
}

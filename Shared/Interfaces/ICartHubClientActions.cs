using GroceryListHelper.Shared.Models.BaseModels;
using GroceryListHelper.Shared.Models.CartProducts;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubClientActions
{
    Task<HubResponse> CreateGroup(List<string> allowedUserEmails);
    Task<HubResponse> JoinGroup(string hostEmail);
    Task<HubResponse> LeaveGroup();
    Task<HubResponse> CartItemAdded(CartProduct product);
    Task<HubResponse> CartItemModified(CartProductCollectable product);
    Task<HubResponse> CartItemDeleted(Guid id);
}

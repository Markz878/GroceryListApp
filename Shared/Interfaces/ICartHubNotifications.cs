using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubNotifications
{
    Task ReceiveCart(List<CartProductCollectable> cartProducts);
    Task LeaveCart(string hostEmail);
    Task ItemAdded(CartProductCollectable cartProduct);
    Task ItemModified(CartProductCollectable cartProduct);
    Task ItemDeleted(string id);
    Task GetMessage(string message);
}

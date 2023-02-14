using GroceryListHelper.Shared.Models.CartProducts;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubNotifications
{
    Task ReceiveCart(List<CartProductCollectable> cartProducts);
    Task ItemAdded(CartProductCollectable cartProduct);
    Task ItemModified(CartProductCollectable cartProduct);
    Task ItemDeleted(string name);
    Task GetMessage(string message);
}

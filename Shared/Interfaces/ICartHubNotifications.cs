namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubNotifications
{
    Task ReceiveCart(List<CartProductCollectable> cartProducts);
    Task LeaveCart(string hostEmail);
    Task ItemAdded(CartProductCollectable cartProduct);
    Task ItemModified(CartProductCollectable cartProduct);
    Task ItemCollected(string id);
    Task ItemDeleted(string id);
    Task ItemMoved(string id, int newIndex);
    Task GetMessage(string message);
}

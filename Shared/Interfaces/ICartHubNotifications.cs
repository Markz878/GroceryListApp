using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubNotifications
{
    Task ReceiveCart(List<CartProductCollectable> cartProducts);
    Task LeaveCart(string hostEmail);
    Task ItemAdded(CartProductCollectable cartProduct);
    Task ItemModified(CartProductCollectable cartProduct);
    Task ItemCollected(int id);
    Task ItemDeleted(int id);
    Task ItemMoved(int id, int newIndex);
    Task GetMessage(string message);
}

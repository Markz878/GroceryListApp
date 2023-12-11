using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubNotifications
{
    Task ProductAdded(CartProduct cartProduct);
    Task ProductModified(CartProductCollectable cartProduct);
    Task ProductsDeleted();
    Task ProductDeleted(string name);
    Task GetMessage(string message);
    Task ProductsSorted(ListSortDirection sortDirection);
}

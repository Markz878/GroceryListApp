using GroceryListHelper.Core.Domain.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.Server.Interfaces;

public interface ICartHubNotifications
{
    Task ProductAdded(CartProduct cartProduct);
    Task ProductModified(CartProduct cartProduct);
    Task ProductsDeleted();
    Task ProductDeleted(string name);
    Task GetMessage(string message);
    Task ProductsSorted(ListSortDirection sortDirection);
}

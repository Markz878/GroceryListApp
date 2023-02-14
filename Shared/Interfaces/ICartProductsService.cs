using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task DeleteAllCartProducts();
    Task DeleteCartProduct(string name);
    Task SaveCartProduct(CartProduct product);
    Task UpdateCartProduct(CartProductCollectable cartProduct);
    Task SortCartProducts(ListSortDirection sortDirection);
}
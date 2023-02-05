using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task DeleteAllCartProducts();
    Task DeleteCartProduct(Guid id);
    Task<Guid> SaveCartProduct(CartProduct product);
    Task UpdateCartProduct(CartProductUIModel cartProduct);
    Task SortCartProducts(ListSortDirection sortDirection);
}
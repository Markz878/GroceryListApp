using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.Client.Services;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task DeleteAllCartProducts();
    Task DeleteCartProduct(string id);
    Task<string> SaveCartProduct(CartProduct product);
    Task UpdateCartProduct(CartProductUIModel cartProduct);
}
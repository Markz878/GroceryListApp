using GroceryListHelper.Client.Models;

namespace GroceryListHelper.Client.Services;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task DeleteAllCartProducts();
    Task DeleteCartProduct(string id);
    Task SaveCartProduct(CartProductUIModel product);
    Task UpdateCartProduct(CartProductUIModel cartProduct);
}
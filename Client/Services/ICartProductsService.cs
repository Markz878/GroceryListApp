using GroceryListHelper.Client.Models;

namespace GroceryListHelper.Client.Services;

public interface ICartProductsService
{
    Task<List<CartProductUIModel>> GetCartProducts();
    Task<bool> ClearCartProducts();
    Task<bool> DeleteCartProduct(string id);
    Task<bool> MarkCartProductCollected(string id);
    Task<bool> SaveCartProduct(CartProductUIModel product);
    Task<bool> UpdateCartProduct(CartProductUIModel cartProduct);
}
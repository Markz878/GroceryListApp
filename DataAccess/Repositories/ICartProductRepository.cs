using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    Task<string> AddCartProduct(CartProduct cartProduct, string userId);
    Task<bool> DeleteItem(string productId, string userId);
    Task<List<CartProductCollectable>> GetCartProductsForUser(string userId);
    Task<CartProductCollectable> GetCartProductForUser(string productId, string userId);
    Task<bool> MarkAsCollected(string productId, string userId);
    Task RemoveItemsForUser(string userId);
    Task<bool> UpdateProduct(string productId, string userId, CartProduct updatedProduct);
}

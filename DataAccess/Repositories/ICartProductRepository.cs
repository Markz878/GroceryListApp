using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    /// <summary>
    /// Add a product to the repository.
    /// </summary>
    /// <returns>The added item's id.</returns>
    Task<string> AddCartProduct(CartProduct cartProduct, string userId);
    /// <summary>
    /// Delete a product from repository.
    /// </summary>
    /// <returns>Flag indicating if deletion succeeded.</returns>
    Task<bool> DeleteProduct(string productId, string userId);
    /// <summary>
    /// Get cart product list for a user.
    /// </summary>
    Task<List<CartProductCollectable>> GetCartProductsForUser(string userId);
    /// <summary>
    /// Get a cart product for a user.
    /// </summary>
    Task<CartProductCollectable> GetCartProductForUser(string productId, string userId);
    /// <summary>
    /// Clear user's cart products.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task ClearProductsForUser(string userId);
    /// <summary>
    /// Update a user's product info.
    /// </summary>
    /// <returns>Flag indicating if update succeeded.</returns>
    Task<bool> UpdateProduct(string userId, CartProductCollectable updatedProduct);
}

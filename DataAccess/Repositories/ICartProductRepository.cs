using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    /// <summary>
    /// Add a product for a user.
    /// </summary>
    /// <returns>The added item's id.</returns>
    Task<Guid> AddCartProduct(CartProduct cartProduct, Guid userId);
    /// <summary>
    /// Delete a user's product.
    /// </summary>
    Task<Exception?> DeleteProduct(Guid productId, Guid userId);
    /// <summary>
    /// Get cart product list for a user.
    /// </summary>
    Task<List<CartProductCollectable>> GetCartProductsForUser(Guid userId);
    /// <summary>
    /// Clear user's cart products.
    /// </summary>
    Task ClearProductsForUser(Guid userId);
    /// <summary>
    /// Update a user's product info.
    /// </summary>
    Task<Exception?> UpdateProduct(Guid userId, CartProductCollectable updatedProduct);
    Task SortUserProducts(Guid userId, ListSortDirection sortDirection);
}

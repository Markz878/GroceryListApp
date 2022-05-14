using GroceryListHelper.Shared.Exceptions;
using GroceryListHelper.Shared.Models.CartProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    /// <summary>
    /// Add a product to the repository.
    /// </summary>
    /// <returns>The added item's id.</returns>
    Task<Guid> AddCartProduct(CartProduct cartProduct, Guid userId);
    /// <summary>
    /// Delete a product from repository.
    /// </summary>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="ForbiddenException"></exception>
    Task DeleteProduct(Guid productId, Guid userId);
    /// <summary>
    /// Get cart product list for a user.
    /// </summary>
    Task<List<CartProductCollectable>> GetCartProductsForUser(Guid userId);
    /// <summary>
    /// Get a cart product for a user.
    /// </summary>
    Task<CartProductCollectable> GetCartProductForUser(Guid productId, Guid userId);
    /// <summary>
    /// Clear user's cart products.
    /// </summary>
    Task ClearProductsForUser(Guid userId);
    /// <summary>
    /// Update a user's product info.
    /// </summary>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="ForbiddenException"></exception>
    Task UpdateProduct(Guid userId, CartProductCollectable updatedProduct);
}

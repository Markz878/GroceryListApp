using GroceryListHelper.Core.Exceptions;
using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.Core.RepositoryContracts;

public interface ICartProductRepository
{
    Task<ConflictError?> AddCartProduct(CartProduct cartProduct, Guid ownerId);
    Task<NotFoundError?> DeleteProduct(string productName, Guid ownerId);
    Task<List<CartProductCollectable>> GetCartProducts(Guid ownerId);
    Task ClearCartProducts(Guid ownerId);
    Task<NotFoundError?> UpdateProduct(Guid ownerId, CartProductCollectable updatedProduct);
    Task SortCartProducts(Guid ownerId, ListSortDirection sortDirection);
}

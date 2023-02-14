using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    Task AddCartProduct(CartProduct cartProduct, Guid ownerId);
    Task<Exception?> DeleteProduct(string productName, Guid ownerId);
    Task<List<CartProductCollectable>> GetCartProducts(Guid ownerId);
    Task ClearCartProducts(Guid ownerId);
    Task<Exception?> UpdateProduct(Guid ownerId, CartProductCollectable updatedProduct);
    Task SortUserProducts(Guid ownerId, ListSortDirection sortDirection);
}

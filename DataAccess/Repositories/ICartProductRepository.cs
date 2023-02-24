using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    Task AddCartProduct(CartProduct cartProduct, Guid ownerId);
    Task<NotFound?> DeleteProduct(string productName, Guid ownerId);
    Task<List<CartProductCollectable>> GetCartProducts(Guid ownerId);
    Task ClearCartProducts(Guid ownerId);
    Task<NotFound?> UpdateProduct(Guid ownerId, CartProductCollectable updatedProduct);
    Task SortCartProducts(Guid ownerId, ListSortDirection sortDirection);
}

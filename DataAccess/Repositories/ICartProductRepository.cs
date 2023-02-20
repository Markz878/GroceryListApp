using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    Task AddCartProduct(CartProduct cartProduct, Guid ownerId);
    Task<NotFoundException?> DeleteProduct(string productName, Guid ownerId);
    Task<List<CartProductCollectable>> GetCartProducts(Guid ownerId);
    Task ClearCartProducts(Guid ownerId);
    Task<NotFoundException?> UpdateProduct(Guid ownerId, CartProductCollectable updatedProduct);
    Task SortCartProducts(Guid ownerId, ListSortDirection sortDirection);
}

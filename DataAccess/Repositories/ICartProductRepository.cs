using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Models.CartProduct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartProductRepository
{
    Task<int> AddCartProduct(CartProduct cartProduct, int userId);
    Task<bool> DeleteItem(int productId, int userId);
    Task<List<CartProductCollectable>> GetCartProductsForUser(int userId);
    Task<CartProductCollectable> GetCartProductForUser(int productId, int userId);
    Task<bool> MarkAsCollected(int productId, int userId);
    Task RemoveItemsForUser(int userId);
    Task<bool> UpdateProduct(int productId, int userId, CartProduct updatedProduct);
}

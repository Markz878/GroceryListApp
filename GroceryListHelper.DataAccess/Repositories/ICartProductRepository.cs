using GroceryListHelper.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories
{
    public interface ICartProductRepository
    {
        Task<int> AddCartProduct(CartProduct cartProduct, int userId);
        Task<bool> DeleteItem(int productId, int userId);
        Task<IEnumerable<CartProductCollectable>> GetCartProductsForUser(int userId);
        Task<bool> MarkAsCollected(int productId, int userId);
        Task RemoveItemsForUser(int userId);
        Task<bool> UpdateProduct(int productId, int userId, CartProduct updatedProduct);
    }
}
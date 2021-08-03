using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Shared.Interfaces
{
    public interface ICartProductsRepository
    {
        Task<IEnumerable<CartProductCollectable>> GetCartProductsForUser(int userId);
        Task<int> AddCartProduct(CartProduct cartProduct, int userId);
        Task RemoveItemsForUser(int v);
    }
}

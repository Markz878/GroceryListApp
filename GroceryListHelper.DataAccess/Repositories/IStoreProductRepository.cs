using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories
{
    public interface IStoreProductRepository
    {
        Task<int> AddProduct(StoreProduct product, int userId);
        Task DeleteAll(int userId);
        Task<bool> DeleteItem(int productId, int userId);
        IAsyncEnumerable<StoreProductDbModel> GetStoreProductsForUser(int userId);
        Task<bool> UpdatePrice(int productId, int userId, double price);
    }
}
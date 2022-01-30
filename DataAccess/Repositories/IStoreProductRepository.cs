using GroceryListHelper.Shared.Models.StoreProduct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<int> AddProduct(StoreProductModel product, int userId);
    Task DeleteAll(int userId);
    Task<bool> DeleteItem(int productId, int userId);
    IAsyncEnumerable<StoreProductResponseModel> GetStoreProductsForUser(int userId);
    Task<bool> UpdatePrice(int productId, int userId, double price);
}

using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<string> AddProduct(StoreProductModel product, string userId);
    Task DeleteAll(string userId);
    Task<bool> DeleteItem(string productId, string userId);
    IAsyncEnumerable<StoreProductResponseModel> GetStoreProductsForUser(string userId);
    Task<bool> UpdatePrice(string productId, string userId, double price);
}

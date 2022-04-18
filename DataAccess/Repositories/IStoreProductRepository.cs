using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<StoreProductServerModel> GetStoreProductForUser(Guid productId, string userId);
    Task<List<StoreProductServerModel>> GetStoreProductsForUser(string userId);
    Task<Guid> AddProduct(StoreProductModel product, string userId);
    Task DeleteAll(string userId);
    Task<bool> DeleteItem(Guid productId, string userId);
    Task<bool> UpdatePrice(Guid productId, string userId, double price);
}

using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<StoreProductServerModel> GetStoreProductForUser(Guid productId, Guid userId);
    Task<List<StoreProductServerModel>> GetStoreProductsForUser(Guid userId);
    Task<Guid> AddProduct(StoreProductModel product, Guid userId);
    Task DeleteAll(Guid userId);
    Task<bool> DeleteItem(Guid productId, Guid userId);
    Task<bool> UpdatePrice(Guid productId, Guid userId, double price);
}

using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<List<StoreProductServerModel>> GetStoreProductsForUser(Guid userId);
    Task<Guid> AddProduct(StoreProduct product, Guid userId);
    Task DeleteAll(Guid userId);
    Task<Exception?> UpdatePrice(Guid productId, Guid userId, double price);
}

using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<List<StoreProductServerModel>> GetStoreProductsForUser(Guid userId);
    Task<Guid> AddProduct(StoreProductModel product, Guid userId);
    Task DeleteAll(Guid userId);
    Task<Exception?> DeleteItem(Guid productId, Guid userId);
    Task<Exception?> UpdatePrice(Guid productId, Guid userId, double price);
}

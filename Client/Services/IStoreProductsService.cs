using GroceryListHelper.Client.Models;

namespace GroceryListHelper.Client.Services;

public interface IStoreProductsService
{
    Task<bool> ClearStoreProducts();
    Task<List<StoreProductUIModel>> GetStoreProducts();
    Task<bool> SaveStoreProduct(StoreProductUIModel product);
    Task<bool> UpdateStoreProductPrice(StoreProductUIModel storeProduct);
}
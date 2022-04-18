using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.StoreProduct;

namespace GroceryListHelper.Client.Services;

public interface IStoreProductsService
{
    Task<bool> ClearStoreProducts();
    Task<List<StoreProductUIModel>> GetStoreProducts();
    Task<string> SaveStoreProduct(StoreProductModel product);
    Task<bool> UpdateStoreProduct(StoreProductUIModel storeProduct);
}
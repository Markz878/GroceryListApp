using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.Shared.Interfaces;

public interface IStoreProductsService
{
    Task ClearStoreProducts();
    Task<List<StoreProduct>> GetStoreProducts();
    Task SaveStoreProduct(StoreProduct product);
    Task UpdateStoreProduct(StoreProduct storeProduct);
}
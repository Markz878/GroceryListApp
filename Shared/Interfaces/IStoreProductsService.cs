using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.Shared.Interfaces;

public interface IStoreProductsService
{
    Task<bool> ClearStoreProducts();
    Task<List<StoreProductUIModel>> GetStoreProducts();
    Task<Guid> SaveStoreProduct(StoreProduct product);
    Task<bool> UpdateStoreProduct(StoreProductUIModel storeProduct);
}
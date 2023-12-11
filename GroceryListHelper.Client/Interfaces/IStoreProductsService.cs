namespace GroceryListHelper.Client.Interfaces;

public interface IStoreProductsService
{
    Task<List<StoreProduct>> GetStoreProducts();
    Task ClearStoreProducts();
    Task SaveStoreProduct(StoreProduct product);
    Task UpdateStoreProduct(StoreProduct storeProduct);
}

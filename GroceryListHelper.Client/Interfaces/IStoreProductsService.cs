namespace GroceryListHelper.Client.Interfaces;

public interface IStoreProductsService
{
    Task<List<StoreProduct>> GetStoreProducts();
    Task DeleteStoreProducts();
    Task CreateStoreProduct(StoreProduct product);
    Task UpdateStoreProduct(StoreProduct storeProduct);
}

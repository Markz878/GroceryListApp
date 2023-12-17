namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsLocalService(ILocalStorageService localStorage, AppState appState) : IStoreProductsService
{
    private const string storeProductsKey = "storeProducts";

    public async Task<List<StoreProduct>> GetStoreProducts()
    {
        return await localStorage.GetItemAsync<List<StoreProduct>>(storeProductsKey) ?? [];
    }

    public async Task CreateStoreProduct(StoreProduct product)
    {
        await localStorage.SetItemAsync(storeProductsKey, appState.StoreProducts);
    }

    public async Task DeleteStoreProducts()
    {
        await localStorage.RemoveItemAsync(storeProductsKey);
    }

    public async Task UpdateStoreProduct(StoreProduct storeProduct)
    {
        await localStorage.SetItemAsync(storeProductsKey, appState.StoreProducts);
    }
}

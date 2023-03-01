namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsLocalService : IStoreProductsService
{
    private readonly MainViewModel viewModel;
    private readonly ILocalStorageService localStorage;
    private const string storeProductsKey = "storeProducts";

    public StoreProductsLocalService(ILocalStorageService localStorage, MainViewModel viewModel)
    {
        this.localStorage = localStorage;
        this.viewModel = viewModel;
    }

    public async Task<List<StoreProduct>> GetStoreProducts()
    {
        return await localStorage.GetItemAsync<List<StoreProduct>>(storeProductsKey) ?? new List<StoreProduct>();
    }

    public async Task SaveStoreProduct(StoreProduct product)
    {
        await localStorage.SetItemAsync(storeProductsKey, viewModel.StoreProducts);
    }

    public async Task ClearStoreProducts()
    {
        await localStorage.RemoveItemAsync(storeProductsKey);
    }

    public async Task UpdateStoreProduct(StoreProduct storeProduct)
    {
        await localStorage.SetItemAsync(storeProductsKey, viewModel.StoreProducts);
    }
}

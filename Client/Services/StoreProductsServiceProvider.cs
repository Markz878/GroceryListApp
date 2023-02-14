namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsServiceProvider : IStoreProductsService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private bool isAuthenticated;
    private IStoreProductsService? actingStoreService;

    public StoreProductsServiceProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
    {
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public async Task ClearStoreProducts()
    {
        actingStoreService = await SelectProvider();
        await actingStoreService.ClearStoreProducts();
    }

    public async Task<List<StoreProduct>> GetStoreProducts()
    {
        actingStoreService = await SelectProvider();
        return await actingStoreService.GetStoreProducts();
    }

    public async Task SaveStoreProduct(StoreProduct product)
    {
        actingStoreService = await SelectProvider();
        await actingStoreService.SaveStoreProduct(product);
    }

    public async Task UpdateStoreProduct(StoreProduct product)
    {
        actingStoreService = await SelectProvider();
        await actingStoreService.UpdateStoreProduct(product);
    }

    private async ValueTask<IStoreProductsService> SelectProvider()
    {
        if (actingStoreService is null)
        {
            isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
            actingStoreService = isAuthenticated ? new StoreProductsAPIService(httpClientFactory) : new StoreProductsLocalService(localStorage);
        }
        return actingStoreService;
    }
}

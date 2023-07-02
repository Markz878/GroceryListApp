namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsServiceFactory : IStoreProductsServiceFactory
{
    private readonly IEnumerable<IStoreProductsService> storeProductsServices;
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public StoreProductsServiceFactory(IEnumerable<IStoreProductsService> storeProductsServices, AuthenticationStateProvider authenticationStateProvider)
    {
        this.storeProductsServices = storeProductsServices;
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<IStoreProductsService> GetStoreProductsService()
    {
        bool isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
        return isAuthenticated ? storeProductsServices.OfType<StoreProductsAPIService>().First() : storeProductsServices.OfType<StoreProductsLocalService>().First();
    }
}

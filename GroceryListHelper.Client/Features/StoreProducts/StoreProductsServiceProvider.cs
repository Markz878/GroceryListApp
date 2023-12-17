namespace GroceryListHelper.Client.Features.StoreProducts;

internal class StoreProductsServiceProvider(AuthenticationStateProvider authStateProvider, IServiceProvider serviceProvider)
{
    public async Task<IStoreProductsService> ResolveStoreProductsService()
    {
        AuthenticationState authState = await authStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            return serviceProvider.GetRequiredKeyedService<IStoreProductsService>(ServiceKey.Api);
        }
        else
        {
            return serviceProvider.GetRequiredKeyedService<IStoreProductsService>(ServiceKey.Local);
        }
    }
}

namespace GroceryListHelper.Client.Features.CartProducts;

internal class CartProductsServiceProvider(AuthenticationStateProvider authStateProvider, NavigationManager navigationManager, IServiceProvider serviceProvider)
{
    public async Task<ICartProductsService> ResolveCartProductsService()
    {
        AuthenticationState authState = await authStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            if (navigationManager.Uri.Contains("group"))
            {
                return serviceProvider.GetRequiredKeyedService<ICartProductsService>(ServiceKey.Group);
            }
            else
            {
                return serviceProvider.GetRequiredKeyedService<ICartProductsService>(ServiceKey.Api);
            }
        }
        else
        {
            return serviceProvider.GetRequiredKeyedService<ICartProductsService>(ServiceKey.Local);
        }
    }
}

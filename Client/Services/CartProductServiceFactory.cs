namespace GroceryListHelper.Client.Services;

public sealed class CartProductServiceFactory : ICartProductsServiceFactory
{
    private readonly IEnumerable<ICartProductsService> cartProductServices;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly MainViewModel viewModel;
    private readonly NavigationManager navigation;

    public CartProductServiceFactory(IEnumerable<ICartProductsService> cartProductServices, AuthenticationStateProvider authenticationStateProvider, MainViewModel viewModel, NavigationManager navigation)
    {
        this.cartProductServices = cartProductServices;
        this.authenticationStateProvider = authenticationStateProvider;
        this.viewModel = viewModel;
        this.navigation = navigation;
    }

    public async Task<ICartProductsService> GetCartProductsService()
    {
        bool isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
        if (isAuthenticated)
        {
            if (viewModel.IsPolling)
            {
                return cartProductServices.OfType<CartProductsSignalRService>().First();
            }
            else if (navigation.Uri.Contains("/groupcart/"))
            {
                return cartProductServices.OfType<CartProductsGroupService>().First();
            }
            else
            {
                return cartProductServices.OfType<CartProductsApiService>().First();
            }
        }
        else
        {
            return cartProductServices.OfType<CartProductsLocalService>().First();
        }
    }
}

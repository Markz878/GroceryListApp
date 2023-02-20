using System.ComponentModel;

namespace GroceryListHelper.Client.Services;

public sealed class CartProductsServiceProvider : ICartProductsService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly MainViewModel viewModel;
    private readonly ICartHubClient cartHubClient;
    private readonly NavigationManager navigation;
    private ICartProductsService? actingCartService;

    public CartProductsServiceProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider, MainViewModel viewModel, ICartHubClient cartHubClient, NavigationManager navigation)
    {
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.authenticationStateProvider = authenticationStateProvider;
        this.viewModel = viewModel;
        this.cartHubClient = cartHubClient;
        this.navigation = navigation;
    }

    public async Task DeleteAllCartProducts()
    {
        actingCartService = await SelectProvider();
        await actingCartService.DeleteAllCartProducts();
    }

    public async Task DeleteCartProduct(string name)
    {
        actingCartService = await SelectProvider();
        await actingCartService.DeleteCartProduct(name);
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        actingCartService = await SelectProvider();
        return await actingCartService.GetCartProducts();
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        actingCartService = await SelectProvider();
        await actingCartService.SaveCartProduct(product);
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        actingCartService = await SelectProvider();
        await actingCartService.UpdateCartProduct(cartProduct);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        actingCartService = await SelectProvider();
        await actingCartService.SortCartProducts(sortDirection);
    }

    private async ValueTask<ICartProductsService> SelectProvider()
    {
        bool isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
        if (isAuthenticated)
        {
            if (viewModel.IsPolling)
            {
                actingCartService = new CartProductsSignalRService(cartHubClient);
            }
            else if (navigation.Uri.Contains("group"))
            {
                actingCartService = new CartProductsGroupService(httpClientFactory, navigation);
            }
            else if (!viewModel.IsPolling)
            {
                actingCartService = new CartProductsApiService(httpClientFactory);
            }
        }
        else
        {
            actingCartService = new CartProductsLocalService(viewModel, localStorage);
        }
        ArgumentNullException.ThrowIfNull(actingCartService);
        return actingCartService;
    }
}

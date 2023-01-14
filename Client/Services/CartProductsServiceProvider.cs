namespace GroceryListHelper.Client.Services;

public sealed class CartProductsServiceProvider : ICartProductsService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly IndexViewModel viewModel;
    private readonly ICartHubClient cartHubClient;
    private ICartProductsService? actingCartService;

    public CartProductsServiceProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider, IndexViewModel viewModel, ICartHubClient cartHubClient)
    {
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.authenticationStateProvider = authenticationStateProvider;
        this.viewModel = viewModel;
        this.cartHubClient = cartHubClient;
    }

    public async Task DeleteAllCartProducts()
    {
        actingCartService = await SelectProvider();
        await actingCartService.DeleteAllCartProducts();
    }

    public async Task DeleteCartProduct(Guid id)
    {
        actingCartService = await SelectProvider();
        await actingCartService.DeleteCartProduct(id);
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        actingCartService = await SelectProvider();
        return await actingCartService.GetCartProducts();
    }

    public async Task<Guid> SaveCartProduct(CartProduct product)
    {
        actingCartService = await SelectProvider();
        Guid id = await actingCartService.SaveCartProduct(product);
        return id;
    }

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        actingCartService = await SelectProvider();
        await actingCartService.UpdateCartProduct(cartProduct);
    }

    private async ValueTask<ICartProductsService> SelectProvider()
    {
        bool isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
        if (isAuthenticated)
        {
            if (viewModel.IsPolling && actingCartService is not CartProductsSignalRService)
            {
                actingCartService = new CartProductsSignalRService(cartHubClient);
            }
            else if (!viewModel.IsPolling && actingCartService is not CartProductsApiService)
            {
                actingCartService = new CartProductsApiService(httpClientFactory);
            }
        }
        else if (actingCartService is not CartProductsLocalService)
        {
            actingCartService = new CartProductsLocalService(localStorage);
        }
        ArgumentNullException.ThrowIfNull(actingCartService);
        return actingCartService;
    }
}

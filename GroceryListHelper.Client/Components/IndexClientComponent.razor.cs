namespace GroceryListHelper.Client.Components;
public sealed partial class IndexClientComponent : IDisposable
{
    [Parameter][EditorRequired] public required List<CartProductCollectable> CartProducts { get; init; }
    [Parameter][EditorRequired] public required List<StoreProduct> StoreProducts { get; init; }
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }
    [Inject] public required IServiceProvider ServiceProvider { get; init; }
    [Inject] public required RenderLocation RenderLocation { get; init; }
    [Inject] public required AppState AppState { get; init; }

    private ICartProductsService? _cartProductsService;
    private IStoreProductsService? _storeProductsService;
    private bool _showLoading;

    protected override async Task OnInitializedAsync()
    {
        UserInfo user = await AuthenticationStateTask.GetUserInfo();
        AppState.CartProducts.Clear();
        AppState.StoreProducts.Clear();
        if (RenderLocation is ClientRenderLocation)
        {
            if (user.IsAuthenticated)
            {
                _cartProductsService = ServiceProvider.GetRequiredKeyedService<ICartProductsService>(ServiceKey.Api);
                _storeProductsService = ServiceProvider.GetRequiredKeyedService<IStoreProductsService>(ServiceKey.Api);
            }
            else
            {
                _cartProductsService = ServiceProvider.GetRequiredKeyedService<ICartProductsService>(ServiceKey.Local);
                _storeProductsService = ServiceProvider.GetRequiredKeyedService<IStoreProductsService>(ServiceKey.Local);
                List<CartProductCollectable> cartProducts = await _cartProductsService.GetCartProducts();
                CartProducts.AddRange(cartProducts);
                List<StoreProduct> storeProducts = await _storeProductsService.GetStoreProducts();
                StoreProducts.AddRange(storeProducts);
            }
            AppState.StateChanged += StateChanged;
        }
        _showLoading = RenderLocation is ServerRenderedLocation && user.IsAuthenticated is false;
        foreach (CartProductCollectable cartProduct in CartProducts)
        {
            AppState.CartProducts.Add(cartProduct);
        }
        foreach (StoreProduct storeProduct in StoreProducts)
        {
            AppState.StoreProducts.Add(storeProduct);
        }
    }

    private Task StateChanged()
    {
#if DEBUG
        Console.WriteLine(nameof(StateChanged));
#endif
        return InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (AppState is not null)
        {
            AppState.StateChanged -= StateChanged;
        }
    }
}
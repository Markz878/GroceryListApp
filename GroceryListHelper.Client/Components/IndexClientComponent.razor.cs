using GroceryListHelper.Client.Features.CartProducts;
using GroceryListHelper.Client.Features.StoreProducts;

namespace GroceryListHelper.Client.Components;
public sealed partial class IndexClientComponent : IDisposable
{
    [Parameter][EditorRequired] public required List<CartProductCollectable> CartProducts { get; init; }
    [Parameter][EditorRequired] public required List<StoreProduct> StoreProducts { get; init; }
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }
    [Inject] public required IMediator Mediator { get; init; }
    [Inject] public required RenderLocation RenderLocation { get; init; }
    [Inject] public required AppState AppState { get; init; }

    private bool _showLoading;

    protected override async Task OnInitializedAsync()
    {
        AppState.CartProducts.Clear();
        foreach (CartProductCollectable cartProduct in CartProducts)
        {
            AppState.CartProducts.Add(cartProduct);
        }
        AppState.StoreProducts.Clear();
        foreach (StoreProduct storeProduct in StoreProducts)
        {
            AppState.StoreProducts.Add(storeProduct);
        }
        UserInfo user = await AuthenticationStateTask.GetUserInfo();
        if (RenderLocation is ClientRenderLocation && !user.IsAuthenticated)
        {
            List<CartProductCollectable> cartProducts = await Mediator.Send(new GetCartProductsQuery());
            CartProducts.AddRange(cartProducts);
            List<StoreProduct> storeProducts = await Mediator.Send(new GetStoreProductsQuery());
            StoreProducts.AddRange(storeProducts);
            foreach (CartProductCollectable cartProduct in CartProducts)
            {
                AppState.CartProducts.Add(cartProduct);
            }
            foreach (StoreProduct storeProduct in StoreProducts)
            {
                AppState.StoreProducts.Add(storeProduct);
            }
        }
        _showLoading = RenderLocation is ServerRenderedLocation && user.IsAuthenticated is false;
        if(RenderLocation is ClientRenderLocation)
        {
            AppState.StateChanged += StateChanged;
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
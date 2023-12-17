using GroceryListHelper.Shared.Models.CartGroups;
using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Client.Components;

[Authorize]
public sealed partial class CartGroupComponent : IAsyncDisposable
{
    [Parameter][EditorRequired] public required CartGroup CartGroup { get; init; }
    [Parameter][EditorRequired] public required List<CartProductCollectable> CartProducts { get; init; }
    [Parameter][EditorRequired] public required List<StoreProduct> StoreProducts { get; init; }
    [Inject] public required IServiceProvider ServiceProvider { get; init; }
    [Inject] public required AppState AppState { get; init; }
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }
    [Inject] public required RenderLocation RenderLocation { get; init; }

    private ICartHubClient? _cartHubClient;

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
        if (RenderLocation is ClientRenderLocation)
        {
            _cartHubClient = ServiceProvider.GetRequiredService<ICartHubClient>();
            await _cartHubClient.JoinGroup(CartGroup.Id);
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

    public async ValueTask DisposeAsync()
    {
        if (AppState is not null)
        {
            AppState.StateChanged -= StateChanged;
        }
        if (_cartHubClient is not null)
        {
            await _cartHubClient.LeaveGroup(CartGroup.Id);
        }
    }
}
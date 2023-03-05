using Microsoft.AspNetCore.Http.Connections.Client;

namespace GroceryListHelper.Client.HelperMethods;

public sealed class CartHubClient : ICartHubClient
{
    private readonly MainViewModel indexViewModel;
    private readonly ModalViewModel modalViewModel;
    private readonly HubConnection hub;

    public CartHubClient(Uri hubUri, MainViewModel indexViewModel, ModalViewModel modalViewModel, Action<HttpConnectionOptions>? configureHttpConnection = null)
    {
        this.indexViewModel = indexViewModel;
        this.modalViewModel = modalViewModel;
        hub = BuildCartHubConnection(hubUri, configureHttpConnection);
    }

    private HubConnection BuildCartHubConnection(Uri hubUri, Action<HttpConnectionOptions>? configureHttpConnection = null)
    {
        HubConnection hub = configureHttpConnection is null ?
            new HubConnectionBuilder().WithUrl(hubUri).WithAutomaticReconnect().Build() :
            new HubConnectionBuilder().WithUrl(hubUri, configureHttpConnection).WithAutomaticReconnect().Build();

        hub.Closed += CartHub_Closed!;
        hub.Reconnected += CartHub_Reconnected!;
        hub.Reconnecting += CartHub_Reconnecting!;
        hub.On<string>(nameof(ICartHubNotifications.GetMessage), (message) =>
        {
            indexViewModel.ShareCartInfo = message;
        });

        hub.On<List<CartProductCollectable>>(nameof(ICartHubNotifications.ReceiveCart), (cartProducts) =>
        {
            indexViewModel.CartProducts.Clear();
            foreach (CartProductCollectable item in cartProducts)
            {
                indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = item.Amount, IsCollected = item.IsCollected, Name = item.Name, UnitPrice = item.UnitPrice, Order = item.Order });
            }
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemAdded), (p) =>
        {
            indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice, Order = p.Order });
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemModified), (cartProduct) =>
        {
            CartProductUIModel product = indexViewModel.CartProducts.First(x => x.Name == cartProduct.Name);
            product.Amount = cartProduct.Amount;
            product.UnitPrice = cartProduct.UnitPrice;
            product.Order = cartProduct.Order;
            product.IsCollected = cartProduct.IsCollected;
            product.Name = cartProduct.Name;
            indexViewModel.OnPropertyChanged();
        });

        hub.On<string>(nameof(ICartHubNotifications.ItemDeleted), (name) =>
        {
            indexViewModel.CartProducts.Remove(indexViewModel.CartProducts.First(x => x.Name == name));
        });
        return hub;
    }

    private Task CartHub_Reconnecting(Exception arg)
    {
        modalViewModel.ShowInfo($"Cart sharing reconnecting, reason: {arg.Message}");
        return Task.CompletedTask;
    }

    private Task CartHub_Reconnected(string arg)
    {
        modalViewModel.ShowInfo($"Cart sharing reconnected, reason: {arg}");
        return Task.CompletedTask;
    }

    private Task CartHub_Closed(Exception arg)
    {
        modalViewModel.ShowInfo($"Cart sharing connection closed unexpectedly, reason: {arg.Message}");
        return Task.CompletedTask;
    }

    public async ValueTask Start()
    {
        if (hub.State == 0)
        {
            await hub.StartAsync();
        }
    }

    public async ValueTask Stop()
    {
        if (hub.State > 0)
        {
            await hub.StopAsync();
        }
    }

    public Task<HubResponse> JoinGroup(Guid groupId)
    {
        return hub.InvokeAsync<HubResponse>(nameof(JoinGroup), groupId);
    }

    public Task<HubResponse> LeaveGroup(Guid groupId)
    {
        return hub.InvokeAsync<HubResponse>(nameof(LeaveGroup), groupId);
    }

    public Task<HubResponse> CartItemAdded(CartProduct product)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CartItemAdded), product);
    }

    public Task<HubResponse> CartItemModified(CartProductCollectable product)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CartItemModified), product);
    }

    public Task<HubResponse> CartItemDeleted(string name)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CartItemDeleted), name);
    }

    public ValueTask DisposeAsync()
    {
        hub.Closed -= CartHub_Closed!;
        hub.Reconnected -= CartHub_Reconnected!;
        hub.Reconnecting -= CartHub_Reconnecting!;
        return hub.DisposeAsync();
    }
}

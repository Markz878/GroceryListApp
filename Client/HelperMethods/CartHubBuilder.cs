namespace GroceryListHelper.Client.HelperMethods;

public sealed partial class CartHubClient : ICartHubClient
{
    private readonly NavigationManager navigation;
    private readonly MainViewModel indexViewModel;
    private readonly ModalViewModel modalViewModel;
    private readonly ILogger<CartHubClient> logger;
    private readonly HubConnection hub;

    public CartHubClient(NavigationManager navigation, MainViewModel indexViewModel, ModalViewModel modalViewModel, ILogger<CartHubClient> logger)
    {
        this.navigation = navigation;
        this.indexViewModel = indexViewModel;
        this.modalViewModel = modalViewModel;
        this.logger = logger;
        hub = BuildCartHubConnection();
    }

    private HubConnection BuildCartHubConnection()
    {
        HubConnection hub = new HubConnectionBuilder().WithUrl(navigation.ToAbsoluteUri("/carthub")).WithAutomaticReconnect().Build();

        hub.Closed += CartHub_Closed!;
        hub.Reconnected += CartHub_Reconnected!;
        hub.Reconnecting += CartHub_Reconnecting!;

        hub.On<string>(nameof(ICartHubNotifications.GetMessage), (message) =>
        {
            LogGetMessage(message);
            indexViewModel.ShareCartInfo = message;
        });

        hub.On<List<CartProductCollectable>>(nameof(ICartHubNotifications.ReceiveCart), (cartProducts) =>
        {
            LogReceiveCart(cartProducts.Count);
            indexViewModel.CartProducts.Clear();
            foreach (CartProductCollectable item in cartProducts)
            {
                indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = item.Amount, IsCollected = item.IsCollected, Name = item.Name, UnitPrice = item.UnitPrice, Order = item.Order });
            }
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemAdded), (p) =>
        {
            LogItemAdded(p.Name);
            indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice, Order = p.Order });
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemModified), (cartProduct) =>
        {
            LogItemModified(cartProduct.Name);
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
            LogItemDeleted(name);
            indexViewModel.CartProducts.Remove(indexViewModel.CartProducts.First(x => x.Name == name));
        });
        return hub;
    }

    private Task CartHub_Reconnecting(Exception arg)
    {
        modalViewModel.Message = $"Cart sharing reconnecting, reason: {arg.Message}";
        return Task.CompletedTask;
    }

    private Task CartHub_Reconnected(string arg)
    {
        modalViewModel.Message = $"Cart sharing reconnected, reason: {arg}";
        return Task.CompletedTask;
    }

    private Task CartHub_Closed(Exception arg)
    {
        modalViewModel.Message = $"Cart sharing connection closed unexpectedly, reason: {arg.Message}";
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

    [LoggerMessage(0, LogLevel.Information, "Received message '{message}'")]
    partial void LogGetMessage(string message);

    [LoggerMessage(1, LogLevel.Information, "Received cart from server, items count is {cartProductsCount}.")]
    partial void LogReceiveCart(int cartProductsCount);

    [LoggerMessage(2, LogLevel.Information, "Received new item with name {name}.")]
    partial void LogItemAdded(string name);

    [LoggerMessage(3, LogLevel.Information, "Item with name {name} was modified.")]
    partial void LogItemModified(string name);

    [LoggerMessage(4, LogLevel.Information, "Item with name {name} was deleted.")]
    partial void LogItemDeleted(string name);

}

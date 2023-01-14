namespace GroceryListHelper.Client.HelperMethods;

public sealed partial class CartHubClient : ICartHubClient
{
    private readonly NavigationManager navigation;
    private readonly IndexViewModel indexViewModel;
    private readonly ModalViewModel modalViewModel;
    private readonly ILogger<CartHubClient> logger;
    private readonly HubConnection hub;

    public CartHubClient(NavigationManager navigation, IndexViewModel indexViewModel, ModalViewModel modalViewModel, ILogger<CartHubClient> logger)
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
                indexViewModel.CartProducts.Add(new CartProductUIModel() { Id = item.Id, Amount = item.Amount, IsCollected = item.IsCollected, Name = item.Name, UnitPrice = item.UnitPrice, Order = item.Order });
            }
        });

        hub.On<string>(nameof(ICartHubNotifications.LeaveCart), async (hostEmail) =>
        {
            modalViewModel.Message = $"Cart session ended by host {hostEmail}.";
            await hub.StopAsync();
            indexViewModel.IsPolling = false;
            await Task.Delay(2000);
            navigation.NavigateTo("/", true);
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemAdded), (p) =>
        {
            LogItemAdded(p.Id, p.Name);
            indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, Id = p.Id, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice, Order = p.Order });
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemModified), (cartProduct) =>
        {
            LogItemModified(cartProduct.Id, cartProduct.Name);
            CartProductUIModel product = indexViewModel.CartProducts.First(x => x.Id.Equals(cartProduct.Id));
            product.Amount = cartProduct.Amount;
            product.UnitPrice = cartProduct.UnitPrice;
            product.Order = cartProduct.Order;
            product.IsCollected = cartProduct.IsCollected;
            product.Name = cartProduct.Name;
            indexViewModel.OnPropertyChanged();
        });

        hub.On<Guid>(nameof(ICartHubNotifications.ItemDeleted), (id) =>
        {
            LogItemDeleted(id);
            indexViewModel.CartProducts.Remove(indexViewModel.CartProducts.First(x => x.Id == id));
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

    public Task<HubResponse> JoinGroup(string cartHostEmail)
    {
        return hub.InvokeAsync<HubResponse>(nameof(JoinGroup), cartHostEmail);
    }

    public Task<HubResponse> CreateGroup(List<string> allowedUserEmails)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CreateGroup), allowedUserEmails);
    }

    public Task<HubResponse> LeaveGroup()
    {
        return hub.InvokeAsync<HubResponse>(nameof(LeaveGroup));
    }

    public Task<HubResponse> CartItemAdded(CartProduct product)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CartItemAdded), product);
    }

    public Task<HubResponse> CartItemModified(CartProductCollectable product)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CartItemModified), product);
    }

    public Task<HubResponse> CartItemDeleted(Guid id)
    {
        return hub.InvokeAsync<HubResponse>(nameof(CartItemDeleted), id);
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

    [LoggerMessage(2, LogLevel.Information, "Received new item with id {id} and name {name}.")]
    partial void LogItemAdded(Guid id, string name);

    [LoggerMessage(3, LogLevel.Information, "Item with id {id} and name {name} was modified.")]
    partial void LogItemModified(Guid id, string name);

    [LoggerMessage(4, LogLevel.Information, "Item with id {id} was deleted.")]
    partial void LogItemDeleted(Guid id);
}

using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace GroceryListHelper.Client.HelperMethods;

public sealed class CartHubClient : ICartHubClient
{
    private readonly AppState _appState;
    private readonly HubConnection _hub;

    public CartHubClient(AppState appState, Uri hubUri, Action<HttpConnectionOptions>? configureHttpConnection = null)
    {
        _hub = BuildCartHubConnection(hubUri, configureHttpConnection);
        _appState = appState;
    }

    private HubConnection BuildCartHubConnection(Uri hubUri, Action<HttpConnectionOptions>? configureHttpConnection = null)
    {
        HubConnection hub = new HubConnectionBuilder()
            .WithUrl(hubUri, configureHttpConnection!)
            .AddMessagePackProtocol()
            .WithStatefulReconnect()
            .Build();

        hub.Closed += CartHub_Closed!;
        hub.Reconnected += CartHub_Reconnected!;
        hub.Reconnecting += CartHub_Reconnecting!;

        hub.On<string>(nameof(ICartHubNotifications.GetMessage), (message) =>
        {
            _appState.ShowInfo(message);
        });


        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ProductAdded), (p) =>
        {
            if (!_appState.CartProducts.Any(x => x.Name == p.Name))
            {
                _appState.CartProducts.Add(p);
            }
        });

        hub.On<CartProductCollectable>(nameof(ICartHubNotifications.ProductModified), (cartProduct) =>
        {
            CartProductCollectable? product = _appState.CartProducts.FirstOrDefault(x => x.Name == cartProduct.Name);
            if (product is not null)
            {
                product.Amount = cartProduct.Amount;
                product.UnitPrice = cartProduct.UnitPrice;
                product.IsCollected = cartProduct.IsCollected;
                product.Name = cartProduct.Name;
                if (product.Order != cartProduct.Order)
                {
                    product.Order = cartProduct.Order;
                    _appState.SortDirection = SortState.None;
                }
                _appState.OnPropertyChanged();
            }
        });

        hub.On<string>(nameof(ICartHubNotifications.ProductDeleted), (name) =>
        {
            CartProductCollectable? productToRemove = _appState.CartProducts.FirstOrDefault(x => x.Name == name);
            if (productToRemove is not null)
            {
                _appState.CartProducts.Remove(productToRemove);
            }
        });

        hub.On<ListSortDirection>(nameof(ICartHubNotifications.ProductsSorted), (direction) =>
        {
            ProductSortMethods.SortProducts(_appState.CartProducts, direction);
            _appState.SortDirection = direction == ListSortDirection.Ascending ? SortState.Ascending : SortState.Descending;
            _appState.OnPropertyChanged();
        });

        hub.On(nameof(ICartHubNotifications.ProductsDeleted), () =>
        {
            _appState.CartProducts.Clear();
        });

        return hub;
    }


    public string? GetConnectionId()
    {
        return _hub.ConnectionId;
    }

    private Task CartHub_Reconnecting(Exception ex)
    {
        Console.WriteLine(ex.Message);
        _appState.IsSharing = false;
        return Task.CompletedTask;
    }

    private Task CartHub_Reconnected(string arg)
    {
        _appState.IsSharing = true;
        return Task.CompletedTask;
    }

    private Task CartHub_Closed(Exception ex)
    {
        if (ex is not null)
        {
            Console.WriteLine(ex.Message);
            _appState.IsSharing = false;
        }
        return Task.CompletedTask;
    }

    public async Task JoinGroup(Guid groupId)
    {
        try
        {
            if (_hub.State == HubConnectionState.Disconnected)
            {
                await _hub.StartAsync();
                await _hub.InvokeAsync(nameof(ICartHubClient.JoinGroup), groupId);
                _appState.IsSharing = true;
            }
        }
        catch (Exception ex)
        {
            _appState.ShowError($"Problem connecting: {ex.Message}");
        }
    }

    public async Task LeaveGroup(Guid groupId)
    {
        _hub.Closed -= CartHub_Closed!;
        _hub.Reconnected -= CartHub_Reconnected!;
        _hub.Reconnecting -= CartHub_Reconnecting!;
        await _hub.InvokeAsync(nameof(LeaveGroup), groupId);
        await _hub.StopAsync();
        _appState.IsSharing = false;
    }
}

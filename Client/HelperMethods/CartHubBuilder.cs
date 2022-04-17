using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.CartProduct;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GroceryListHelper.Client.HelperMethods;

public partial class CartHubBuilder : IDisposable
{
    private readonly NavigationManager navigation;
    private readonly IndexViewModel indexViewModel;
    private readonly ModalViewModel modalViewModel;
    private readonly ILogger<CartHubBuilder> logger;

    public CartHubBuilder(NavigationManager navigation, IndexViewModel indexViewModel, ModalViewModel modalViewModel, ILogger<CartHubBuilder> logger)
    {
        this.navigation = navigation;
        this.indexViewModel = indexViewModel;
        this.modalViewModel = modalViewModel;
        this.logger = logger;
    }

    public void BuildCartHubConnection()
    {
        indexViewModel.CartHub = new HubConnectionBuilder().WithUrl(navigation.ToAbsoluteUri("/carthub")).WithAutomaticReconnect().Build();

        indexViewModel.CartHub.Closed += CartHub_Closed;
        indexViewModel.CartHub.Reconnected += CartHub_Reconnected;
        indexViewModel.CartHub.Reconnecting += CartHub_Reconnecting;

        indexViewModel.CartHub.On<string>(nameof(ICartHubNotifications.GetMessage), (message) =>
        {
            LogGetMessage(message);
            indexViewModel.ShareCartInfo = message;
        });

        indexViewModel.CartHub.On<List<CartProductCollectable>>(nameof(ICartHubNotifications.ReceiveCart), (cartProducts) =>
        {
            LogReceiveCart(cartProducts.Count);
            indexViewModel.CartProducts.Clear();
            foreach (CartProductCollectable item in cartProducts)
            {
                indexViewModel.CartProducts.Add(new CartProductUIModel() { Id = item.Id, Amount = item.Amount, IsCollected = item.IsCollected, Name = item.Name, UnitPrice = item.UnitPrice, Order = item.Order });
            }
        });

        indexViewModel.CartHub.On<string>(nameof(ICartHubNotifications.LeaveCart), async (hostEmail) =>
        {
            modalViewModel.Message = $"Cart session ended by host {hostEmail}.";
            await indexViewModel.CartHub.StopAsync();
            indexViewModel.IsPolling = false;
            await Task.Delay(2000);
            navigation.NavigateTo("/", true);
        });

        indexViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemAdded), (p) =>
        {
            LogItemAdded(p.Id, p.Name);
            indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, Id = p.Id, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice, Order = p.Order });
        });

        indexViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemModified), (cartProduct) =>
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

        indexViewModel.CartHub.On<string>(nameof(ICartHubNotifications.ItemDeleted), (id) =>
        {
            LogItemDeleted(id);
            indexViewModel.CartProducts.Remove(indexViewModel.CartProducts.FirstOrDefault(x => x.Id == id));
        });
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

    public void Dispose()
    {
        indexViewModel.CartHub.Closed -= CartHub_Closed;
        indexViewModel.CartHub.Reconnected -= CartHub_Reconnected;
        indexViewModel.CartHub.Reconnecting -= CartHub_Reconnecting;
        GC.SuppressFinalize(this);
    }

    [LoggerMessage(0, LogLevel.Information, "Received message '{message}'")]
    partial void LogGetMessage(string message);

    [LoggerMessage(1, LogLevel.Information, "Received cart from server, items count is {cartProductsCount}.")]
    partial void LogReceiveCart(int cartProductsCount);

    [LoggerMessage(2, LogLevel.Information, "Received new item with id {id} and name {name}.")]
    partial void LogItemAdded(string id, string name);

    [LoggerMessage(3, LogLevel.Information, "Item with id {id} and name {name} was modified.")]
    partial void LogItemModified(string id, string name);

    [LoggerMessage(4, LogLevel.Information, "Item with id {id} was deleted.")]
    partial void LogItemDeleted(string id);
}

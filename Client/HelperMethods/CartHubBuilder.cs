using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using System.Text.Json;

namespace GroceryListHelper.Client.HelperMethods;

public class CartHubBuilder
{
    private readonly HttpClient client;
    private readonly IAccessTokenProvider accessTokenProvider;
    private readonly NavigationManager navigation;
    private readonly IndexViewModel indexViewModel;
    private readonly ModalViewModel modalViewModel;
    private readonly ILogger<CartHubBuilder> logger;

    public CartHubBuilder(IHttpClientFactory httpClientFactory, IAccessTokenProvider accessTokenProvider, NavigationManager navigation, IndexViewModel indexViewModel, ModalViewModel modalViewModel, ILogger<CartHubBuilder> logger)
    {
        client = httpClientFactory.CreateClient("AnonymousClient");
        this.accessTokenProvider = accessTokenProvider;
        this.navigation = navigation;
        this.indexViewModel = indexViewModel;
        this.modalViewModel = modalViewModel;
        this.logger = logger;
    }

    public void BuildCartHubConnection()
    {
        indexViewModel.CartHub = new HubConnectionBuilder().WithUrl(navigation.ToAbsoluteUri("/carthub"), options =>
        {
            options.AccessTokenProvider = async () =>
            {
                logger.LogInformation("Authorizing hub connection...");
                HttpResponseMessage response = await client.GetAsync("api/authentication/refresh");
                if (response.IsSuccessStatusCode)
                {
                    AuthenticationResponseModel loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    return loginResponse.AccessToken;
                }
                return null;
            };
        }).WithAutomaticReconnect().Build();

        indexViewModel.CartHub.On<string>(nameof(ICartHubNotifications.GetMessage), (message) =>
        {
            logger.LogInformation("Received message '{message}'.", message);
            indexViewModel.ShareCartInfo = message;
        });

        indexViewModel.CartHub.On<List<CartProductCollectable>>(nameof(ICartHubNotifications.ReceiveCart), (cartProducts) =>
        {
            logger.LogInformation("Received cart from server, items count is {cartProductsCount}. Items are:", cartProducts.Count);
            foreach (var product in cartProducts)
            {
                logger.LogInformation("{product}", product);
            }
            indexViewModel.CartProducts.Clear();
            foreach (CartProductCollectable item in cartProducts)
            {
                indexViewModel.CartProducts.Add(new CartProductUIModel() { Id = item.Id, Amount = item.Amount, IsCollected = item.IsCollected, Name = item.Name, UnitPrice = item.UnitPrice, Order = item.Order });
            }
        });

        indexViewModel.CartHub.On<string>(nameof(ICartHubNotifications.LeaveCart), async (hostEmail) =>
        {
            logger.LogInformation("Cart session ended by host {hostEmail}.", hostEmail);
            modalViewModel.Message = $"Cart session ended by host {hostEmail}.";
            await indexViewModel.CartHub.StopAsync();
            indexViewModel.IsPolling = false;
            await Task.Delay(2000);
            navigation.NavigateTo("/", true);
        });

        indexViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemAdded), (p) =>
        {
            logger.LogInformation("Received new item with id {pId} and name {pName}.", p.Id, p.Name);
            indexViewModel.CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, Id = p.Id, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice, Order = p.Order });
        });

        indexViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemModified), (cartProduct) =>
        {
            logger.LogInformation("Item {productName} was modified.", cartProduct.Name);
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
            logger.LogInformation("Item with id {id} was deleted.", id);
            indexViewModel.CartProducts.Remove(indexViewModel.CartProducts.FirstOrDefault(x => x.Id == id));
        });
    }
}

using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.Authentication;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.HelperMethods
{
    public class CartHubBuilder
    {
        private readonly HttpClient client;
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly NavigationManager navigation;
        private readonly IndexViewModel indexViewModel;
        private readonly ModalViewModel modalViewModel;

        public CartHubBuilder(IHttpClientFactory httpClientFactory, IAccessTokenProvider accessTokenProvider, NavigationManager navigation, IndexViewModel indexViewModel, ModalViewModel modalViewModel)
        {
            client = httpClientFactory.CreateClient("AnonymousClient");
            this.accessTokenProvider = accessTokenProvider;
            this.navigation = navigation;
            this.indexViewModel = indexViewModel;
            this.modalViewModel = modalViewModel;
        }

        public void BuildCartHubConnection()
        {
            indexViewModel.CartHub = new HubConnectionBuilder().WithUrl(navigation.ToAbsoluteUri("/carthub"), options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    Console.WriteLine("Authorizing hub connection...");
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
                Console.WriteLine($"Received message '{message}'.");
                indexViewModel.ShareCartInfo = message;
            });

            indexViewModel.CartHub.On<List<CartProductCollectable>>(nameof(ICartHubNotifications.ReceiveCart), (cartProducts) =>
            {
                Console.WriteLine($"Received cart from server, items count is {cartProducts.Count}.");
                indexViewModel.CartProducts.Clear();
                foreach (CartProductCollectable item in cartProducts)
                {
                    indexViewModel.CartProducts.Add(item.Adapt<CartProductUIModel>());
                }
            });

            indexViewModel.CartHub.On<string>(nameof(ICartHubNotifications.LeaveCart), async (hostEmail) =>
            {
                Console.WriteLine($"Cart session ended by host {hostEmail}.");
                modalViewModel.Message = $"Cart session ended by host {hostEmail}.";
                await indexViewModel.CartHub.StopAsync();
                indexViewModel.IsPolling = false;
                await Task.Delay(2000);
                navigation.NavigateTo("/", true);
            });

            indexViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemAdded), (p) =>
            {
                Console.WriteLine($"Received new item with id {p.Id} and name {p.Name}.");
                indexViewModel.CartProducts.Add(p.Adapt<CartProductUIModel>());
            });

            indexViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubNotifications.ItemModified), (cartProduct) =>
            {
                Console.WriteLine($"Item {cartProduct.Name} was modified.");
                CartProductUIModel product = indexViewModel.CartProducts.First(x => x.Id.Equals(cartProduct.Id));
                product.Amount = cartProduct.Amount;
                product.UnitPrice = cartProduct.UnitPrice;
                indexViewModel.OnPropertyChanged();
            });

            indexViewModel.CartHub.On<int>(nameof(ICartHubNotifications.ItemCollected), (id) =>
            {
                Console.WriteLine($"Item with id {id} was collected.");
                indexViewModel.CartProducts.First(x => x.Id.Equals(id)).IsCollected ^= true;
                indexViewModel.OnPropertyChanged();
            });

            indexViewModel.CartHub.On<int>(nameof(ICartHubNotifications.ItemDeleted), (id) =>
            {
                Console.WriteLine($"Item with id {id} was deleted.");
                indexViewModel.CartProducts.Remove(indexViewModel.CartProducts.FirstOrDefault(x => x.Id == id));
            });

            indexViewModel.CartHub.On<int, int>(nameof(ICartHubNotifications.ItemMoved), (id, newIndex) =>
            {
                Console.WriteLine($"Item with id {id} was moved to {newIndex}.");
                CartProductUIModel item = indexViewModel.CartProducts.FirstOrDefault(x => x.Id == id);
                int oldIndex = indexViewModel.CartProducts.IndexOf(item);
                indexViewModel.CartProducts.Move(oldIndex, newIndex);
            });
        }
    }
}

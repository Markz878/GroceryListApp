using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    public class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IAccessTokenProvider AccessTokenProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ViewModel.CartProducts.Clear();
            foreach (CartProductUIModel item in await CartProductsService.GetCartProducts())
            {
                ViewModel.CartProducts.Add(item);
            }
            ViewModel.StoreProducts.Clear();
            foreach (StoreProductUIModel item in await StoreProductsService.GetStoreProducts())
            {
                ViewModel.StoreProducts.Add(item);
            }
            BuildHubConnection();
        }

        public void BuildHubConnection()
        {
            if (ViewModel.CartHub != null)
            {
                return;
            }
            ViewModel.CartHub = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/carthub"), options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    string tokenResult = await AccessTokenProvider.RequestAccessToken();
                    return tokenResult;
                };
            }).WithAutomaticReconnect().Build();

            ViewModel.CartHub.On<string>(nameof(ICartHubClient.GetMessage), (message) =>
            {
                Console.WriteLine("Received message " + message);
                ViewModel.ShareCartInfo = message;
                StateHasChanged();
            });

            ViewModel.CartHub.On<List<CartProductCollectable>>(nameof(ICartHubClient.GetCart), (cartProducts) =>
            {
                Console.WriteLine("Received cart from server..");
                ViewModel.CartProducts.Clear();
                foreach (CartProductUIModel item in cartProducts.ConvertAll(x => (CartProductUIModel)x))
                {
                    ViewModel.CartProducts.Add(item);
                }
                StateHasChanged();
            });

            ViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubClient.ItemAdded), (p) =>
            {
                Console.WriteLine("Received new item with id " + p.Id + " and name " + p.Name);
                ViewModel.CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, Id = p.Id, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice });
                StateHasChanged();
            });

            ViewModel.CartHub.On<CartProductCollectable>(nameof(ICartHubClient.ItemModified), (cartProduct) =>
            {
                Console.WriteLine($"Item {cartProduct.Name} was modified");
                CartProductUIModel product = ViewModel.CartProducts.First(x => x.Id.Equals(cartProduct.Id));
                product.Amount = cartProduct.Amount;
                product.UnitPrice = cartProduct.UnitPrice;
                StateHasChanged();
            });

            ViewModel.CartHub.On<int>(nameof(ICartHubClient.ItemCollected), (id) =>
            {
                Console.WriteLine($"Item with id {id} was modified");
                ViewModel.CartProducts.First(x => x.Id.Equals(id)).IsCollected ^= true;
                StateHasChanged();
            });

            ViewModel.CartHub.On<int>(nameof(ICartHubClient.ItemDeleted), (id) =>
            {
                Console.WriteLine($"Item with id {id} was deleted");
                ViewModel.CartProducts.Remove(ViewModel.CartProducts.FirstOrDefault(x => x.Id == id));
                StateHasChanged();
            });
        }

        public async ValueTask DisposeAsync()
        {
            await ViewModel?.CartHub?.StopAsync();
        }
    }
}

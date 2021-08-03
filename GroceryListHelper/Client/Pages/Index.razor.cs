using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    public partial class Index : ComponentBase, IAsyncDisposable
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IAccessTokenProvider AccessTokenProvider { get; set; }
        public List<CartProductUIModel> CartProducts { get; set; } = new List<CartProductUIModel>();
        public List<StoreProductUIModel> StoreProducts { get; set; } = new List<StoreProductUIModel>();
        public string Message { get; set; } = string.Empty;

        public ShareModeType ShareMode { get; set; }
        public List<string> AllowedUsers { get; set; } = new List<string>();
        public string CartHostEmail { get; set; } = string.Empty;
        public string AllowEmail { get; set; } = string.Empty;
        public string ShareCartInfo { get; set; } = string.Empty;

        private HubConnection hubConnection;
        private bool polling;

        protected override async Task OnInitializedAsync()
        {
            CartProducts = await CartProductsService.GetCartProducts();
            StoreProducts = await StoreProductsService.GetStoreProducts();
            BuildHubConnection();
        }

        public void BuildHubConnection()
        {
            hubConnection = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/carthub"), options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    string tokenResult = await AccessTokenProvider.RequestAccessToken();
                    return tokenResult;
                };
            }).WithAutomaticReconnect().Build();

            hubConnection.On<string>(nameof(ICartHubClient.GetMessage), (message) =>
            {
                Console.WriteLine("Received message " + message);
                ShareCartInfo = message;
                StateHasChanged();
            });

            hubConnection.On<List<CartProductCollectable>>(nameof(ICartHubClient.GetCart), (cartProducts) =>
            {
                Console.WriteLine("Received cart from server..");
                CartProducts.Clear();
                CartProducts.AddRange(cartProducts.ConvertAll(x => new CartProductUIModel() { Id = x.Id, Amount = x.Amount, IsCollected = x.IsCollected, Name = x.Name, UnitPrice = x.UnitPrice }));
                StateHasChanged();
            });

            hubConnection.On<CartProductCollectable>(nameof(ICartHubClient.ItemAdded), (p) =>
            {
                Console.WriteLine("Received new item with id " + p.Id + " and name " + p.Name);
                CartProducts.Add(new CartProductUIModel() { Amount = p.Amount, Id = p.Id, IsCollected = p.IsCollected, Name = p.Name, UnitPrice = p.UnitPrice });
                StateHasChanged();
            });

            hubConnection.On<CartProductCollectable>(nameof(ICartHubClient.ItemModified), (cartProduct) =>
            {
                Console.WriteLine($"Item {cartProduct.Name} was modified");
                CartProductUIModel product = CartProducts.First(x => x.Id.Equals(cartProduct.Id));
                product.Amount = cartProduct.Amount;
                product.UnitPrice = cartProduct.UnitPrice;
                StateHasChanged();
            });

            hubConnection.On<int>(nameof(ICartHubClient.ItemCollected), (id) =>
            {
                Console.WriteLine($"Item with id {id} was modified");
                CartProducts.First(x => x.Id.Equals(id)).IsCollected ^= true;
                StateHasChanged();
            });

            hubConnection.On<int>(nameof(ICartHubClient.ItemDeleted), (id) =>
            {
                Console.WriteLine($"Item with id {id} was deleted");
                CartProducts.RemoveAll(x => x.Id.Equals(id));
                StateHasChanged();
            });
        }

        private void MessageChanged(string message)
        {
            Message = message;
        }

        private void SetShareMode(ShareModeType shareMode)
        {
            ShareMode = shareMode;
        }

        private void AddUser()
        {
            AllowedUsers.Add(AllowEmail);
            AllowEmail = string.Empty;
        }

        private void DeleteUser(string user)
        {
            AllowedUsers.Remove(user);
        }

        private async Task ShareCart()
        {
            if (AllowedUsers.Count > 0)
            {
                polling = true;
                await hubConnection.StartAsync();
                HubResponse response = await hubConnection.InvokeAsync<HubResponse>(nameof(ICartHub.CreateGroup), AllowedUsers);
                
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    polling = false;
                    await hubConnection.StopAsync();
                    ShareCartInfo = response.ErrorMessage;
                }
                else if (!string.IsNullOrEmpty(response.SuccessMessage))
                {
                    ShareCartInfo = response.SuccessMessage;
                }
            }
            else
            {
                ShareCartInfo = "There are no allowed users for your cart.";
            }

        }

        private async Task JoinCart()
        {
            if (!string.IsNullOrEmpty(CartHostEmail))
            {
                polling = true;
                await hubConnection.StartAsync();
                HubResponse response = await hubConnection.InvokeAsync<HubResponse>(nameof(ICartHub.JoinGroup), CartHostEmail);
                
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    polling = false;
                    await hubConnection.StopAsync();
                    ShareCartInfo = response.ErrorMessage;
                }
                else if (!string.IsNullOrEmpty(response.SuccessMessage))
                {
                    ShareCartInfo = response.SuccessMessage;
                }
            }
            else
            {
                ShareCartInfo = "Give cart host username/email.";
            }
        }

        private void SetHostEmail(string email)
        {
            CartHostEmail = email;
        }

        private async Task ExitCart()
        {
            try
            {
                HubResponse response = await hubConnection.InvokeAsync<HubResponse>(nameof(ICartHub.LeaveGroup));
                ShareCartInfo = response.ErrorMessage;
            }
            catch (Exception ex)
            {
                ShareCartInfo = ex.Message;
            }
            finally
            {
                await hubConnection.StopAsync();
                polling = false;
            }
        }

        private void ModalCancel()
        {
            Message = string.Empty;
        }

        private Task ClearCartProducts()
        {
            CartProducts.Clear();
            return CartProductsService.ClearCartProducts();
        }

        private Task ClearStoreProducts()
        {
            StoreProducts.Clear();
            return StoreProductsService.ClearStoreProducts();
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection?.StopAsync();
        }
    }
}

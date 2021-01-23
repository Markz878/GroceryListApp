using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    [Authorize]
    public partial class Index : ComponentBase, IAsyncDisposable
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IAccessTokenProvider AccessTokenProvider { get; set; }
        public CartProductUIModel NewProduct { get; set; } = new CartProductUIModel();
        public List<CartProductUIModel> CartProducts { get; set; } = new List<CartProductUIModel>();
        public List<StoreProductUIModel> StoreProducts { get; set; } = new List<StoreProductUIModel>();
        public string Message { get; set; } = string.Empty;

        public ShareModeType ShareMode { get; set; }
        public List<string> AllowedUsers { get; set; } = new List<string>();
        public string CartHostEmail { get; set; } = string.Empty;
        public string AllowEmail { get; set; } = string.Empty;
        public string ShareCartInfo { get; set; } = string.Empty;
        public CartProduct EditingItem { get; set; }

        private ElementReference NewProductNameBox;
        private ElementReference AddProductButton;
        private CartProductValidator cartProductValidator;
        private StoreProductValidator storeProductValidator;
        private HubConnection hubConnection;
        private bool polling;

        protected override async Task OnInitializedAsync()
        {
            CartProducts = await CartProductsService.GetCartProducts();
            StoreProducts = await StoreProductsService.GetCartProducts();
            cartProductValidator = new CartProductValidator(CartProducts);
            storeProductValidator = new StoreProductValidator(StoreProducts);
            BuildHubConnection();
        }

        public void BuildHubConnection()
        {
            hubConnection = new HubConnectionBuilder().WithUrl(Navigation.ToAbsoluteUri("/carthub"), options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var tokenResult = await AccessTokenProvider.RequestAccessToken();
                    tokenResult.TryGetToken(out AccessToken accessToken);
                    return accessToken.Value;
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
                CartProducts.Clear();
                CartProducts.AddRange(cartProducts.ConvertAll(x => new CartProductUIModel() { Id = x.Id, Amount = x.Amount, IsCollected = x.IsCollected, Name = x.Name, UnitPrice = x.UnitPrice }));
                StateHasChanged();
            });

            hubConnection.On<CartProductCollectable>(nameof(ICartHubClient.ItemAdded), (cartProduct) =>
            {
                CartProducts.Add((CartProductUIModel)cartProduct);
                StateHasChanged();
            });

            hubConnection.On<CartProductCollectable>(nameof(ICartHubClient.ItemModified), (cartProduct) =>
            {
                var product = CartProducts.First(x => x.Id.Equals(cartProduct.Id));
                product.Amount = cartProduct.Amount;
                product.UnitPrice = cartProduct.UnitPrice;
                StateHasChanged();
            });

            hubConnection.On<int>(nameof(ICartHubClient.ItemCollected), (id) =>
            {
                CartProducts.First(x => x.Id.Equals(id)).IsCollected ^= true;
                StateHasChanged();
            });

            hubConnection.On<int>(nameof(ICartHubClient.ItemDeleted), (id) =>
            {
                CartProducts.RemoveAll(x => x.Id.Equals(id));
                StateHasChanged();
            });
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
                var response = await hubConnection.InvokeAsync<HubResponse>(nameof(ICartHub.CreateGroup), AllowedUsers);
                ShareCartInfo = response.Message;
                if (!response.IsSuccess)
                {
                    polling = false;
                    await hubConnection.StopAsync();
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
                var response = await hubConnection.InvokeAsync<HubResponse>(nameof(ICartHub.JoinGroup), CartHostEmail);
                ShareCartInfo = response.Message;
                if (!response.IsSuccess)
                {
                    polling = false;
                    await hubConnection.StopAsync();
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
                var response = await hubConnection.InvokeAsync<HubResponse>(nameof(ICartHub.LeaveGroup), CartHostEmail);
                ShareCartInfo = response.Message;
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

        private async Task AddNewProduct()
        {
            Message = string.Join(" ", cartProductValidator.Validate(NewProduct).Errors.Select(x => x.ErrorMessage));
            if (string.IsNullOrEmpty(Message))
            {
                CartProductUIModel newProduct = NewProduct;
                NewProduct = new CartProductUIModel();
                Task task1 = SaveCartProduct(newProduct);
                Task task2 = SaveStoreProduct(newProduct.Name, newProduct.UnitPrice);
                await Task.WhenAll(task1, task2).ContinueWith(x =>
                {
                    if (!x.IsFaulted)
                    {
                        NewProductNameBox.FocusAsync().AsTask();
                    }
                });
            }
        }

        private Task SaveCartProduct(CartProductUIModel product)
        {
            CartProducts.Add(product);
            try
            {
                if (polling)
                {
                    return hubConnection.InvokeAsync<bool>(nameof(ICartHub.CartItemAdded), CartHostEmail, product);
                }
                else
                {
                    return CartProductsService.SaveCartProduct(product);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Task.CompletedTask;
            }
        }

        private Task MarkItemCollected(CartProductUIModel product)
        {
            if (polling)
            {
                return hubConnection.InvokeAsync<bool>(nameof(ICartHub.CartItemCollected), product.Id);
            }
            else
            {
                return CartProductsService.MarkCartProductCollected(product.Id);
            }
        }

        private Task SaveStoreProduct(string productName, double unitPrice)
        {
            StoreProductUIModel product = StoreProducts.Find(x => x.Name == productName);
            if (product != null)
            {
                if (product.UnitPrice != unitPrice)
                {
                    product.UnitPrice = unitPrice;
                    return StoreProductsService.UpdateStoreProductPrice(product.Id, unitPrice);
                }
                else
                {
                    return Task.CompletedTask;
                }
            }
            else
            {
                product = new StoreProductUIModel() { Name = productName, UnitPrice = unitPrice };
                if (storeProductValidator.Validate(product).IsValid)
                {
                    StoreProducts.Add(product);
                    return StoreProductsService.SaveStoreProduct(product);
                }
                return Task.CompletedTask;
            }
        }

        private void StartEditItem(CartProductUIModel product)
        {
            EditingItem = product;
        }

        private Task UpdateCartProduct(CartProductUIModel product)
        {
            Message = string.Join(" ", cartProductValidator.Validate(product).Errors.Select(x => x.ErrorMessage));
            if (string.IsNullOrEmpty(Message))
            {
                EditingItem = null;
                if (polling)
                {
                    return hubConnection.InvokeAsync<bool>(nameof(ICartHub.CartItemModified), CartHostEmail, product);
                }
                else
                {
                    return CartProductsService.UpdateCartProduct(product);
                }
            }
            return Task.CompletedTask;
        }

        private void CancelProductUpdate()
        {
            EditingItem = null;
        }

        private void GetItemPrice()
        {
            var product = StoreProducts.Find(x => x.Name == NewProduct.Name);
            if (product?.UnitPrice > 0)
            {
                NewProduct.UnitPrice = product.UnitPrice;
            }
        }

        private void ModalCancel()
        {
            Message = string.Empty;
        }

        private Task RemoveProduct(CartProductUIModel product)
        {
            CartProducts.Remove(product);
            if (polling)
            {
                return hubConnection.InvokeAsync<bool>(nameof(ICartHub.CartItemDeleted), product.Id);
            }
            else
            {
                return CartProductsService.DeleteCartProduct(product.Id);
            }
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

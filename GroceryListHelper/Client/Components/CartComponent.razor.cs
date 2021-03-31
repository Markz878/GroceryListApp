using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Components
{
    public partial class CartComponent
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        [Parameter] public List<CartProductUIModel> CartProducts { get; set; }
        [Parameter] public List<StoreProductUIModel> StoreProducts { get; set; }
        [Parameter] public EventCallback<string> OnMessageChanged { get; set; }
        [Parameter] public bool Polling { get; set; }
        [Parameter] public HubConnection HubConnection { get; set; } 
        public CartProductUIModel NewProduct { get; set; } = new CartProductUIModel() { Amount = 1 };
        public CartProduct EditingItem { get; set; }

        private ElementReference NewProductNameBox;
        private ElementReference AddProductButton;
        private CartProductValidator cartProductValidator;
        private StoreProductValidator storeProductValidator;

        private async Task AddNewProduct()
        {
            cartProductValidator = new CartProductValidator(CartProducts);
            string message = string.Join(" ", cartProductValidator.Validate(NewProduct).Errors.Select(x => x.ErrorMessage));
            if (string.IsNullOrEmpty(message))
            {
                CartProductUIModel newProduct = NewProduct;
                NewProduct = new CartProductUIModel() { Amount = 1 };
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
            else
            {
                await OnMessageChanged.InvokeAsync(message);
            }
        }

        private async Task SaveCartProduct(CartProductUIModel product)
        {
            CartProducts.Add(product);
            try
            {
                if (Polling)
                {
                    product.Id = await HubConnection.InvokeAsync<int>(nameof(ICartHub.CartItemAdded), product);
                }
                else
                {
                    await CartProductsService.SaveCartProduct(product);
                }
            }
            catch (Exception ex)
            {
                await OnMessageChanged.InvokeAsync(ex.Message);
            }
        }

        private Task MarkItemCollected(CartProductUIModel product)
        {
            if (Polling)
            {
                return HubConnection.SendAsync(nameof(ICartHub.CartItemCollected), product.Id);
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
                storeProductValidator = new StoreProductValidator(StoreProducts);
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

        private async Task UpdateCartProduct(CartProductUIModel product)
        {
            cartProductValidator = new CartProductValidator(CartProducts);
            string message = string.Join(" ", cartProductValidator.Validate(product).Errors.Select(x => x.ErrorMessage));
            if (string.IsNullOrEmpty(message))
            {
                EditingItem = null;
                if (Polling)
                {
                    await HubConnection.SendAsync(nameof(ICartHub.CartItemModified), product);
                }
                else
                {
                    await CartProductsService.UpdateCartProduct(product);
                }
            }
            await OnMessageChanged.InvokeAsync(message);
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

        private Task RemoveProduct(CartProductUIModel product)
        {
            CartProducts.Remove(product);
            if (Polling)
            {
                return HubConnection.SendAsync(nameof(ICartHub.CartItemDeleted), product.Id);
            }
            else
            {
                return CartProductsService.DeleteCartProduct(product.Id);
            }
        }
    }
}

using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    [Authorize]
    public partial class Index
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        public CartProductUIModel NewProduct { get; set; } = new CartProductUIModel();
        public List<CartProductUIModel> CartProducts { get; set; } = new List<CartProductUIModel>();
        public List<StoreProductUIModel> StoreProducts { get; set; } = new List<StoreProductUIModel>();
        public bool AllCollected => !CartProducts.Select(x => x.IsCollected).Contains(false);
        public double Total => CartProducts.Sum(x => x.Total);
        public string Message { get; set; } = string.Empty;
        public CartProduct EditingItem { get; set; }

        private ElementReference NewProductNameBox;
        private ElementReference AddProductButton;
        private CartProductValidator cartProductValidator;
        private StoreProductValidator storeProductValidator;
        private bool polling;

        protected override async Task OnInitializedAsync()
        {
            CartProducts = await CartProductsService.GetCartProducts();
            StoreProducts = await StoreProductsService.GetCartProducts();
            cartProductValidator = new CartProductValidator(CartProducts);
            storeProductValidator = new StoreProductValidator(StoreProducts);
        }

        private async Task PollItems()
        {
            polling = true;
            while (polling)
            {
                CartProducts = await CartProductsService.GetCartProducts();
                cartProductValidator = new CartProductValidator(CartProducts);
                StateHasChanged();
                await Task.Delay(TimeSpan.FromSeconds(5));
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

        private async Task SaveCartProduct(CartProductUIModel product)
        {
            CartProducts.Add(product);
            try
            {
                await CartProductsService.SaveCartProduct(product);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        private async Task MarkItemCollected(CartProductUIModel product)
        {
            await CartProductsService.MarkCartProductCollected(product.Id);
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
                return CartProductsService.UpdateCartProduct(product);
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
            return CartProductsService.DeleteCartProduct(product.Id);
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
    }
}

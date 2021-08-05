using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Components
{
    public class CartComponentBase : BasePage<IndexViewModel>
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        [Inject] public ModalViewModel ModalViewModel { get; set; }
        public CartProductUIModel NewProduct { get; set; } = new CartProductUIModel() { Amount = 1 };
        public CartProduct EditingItem { get; set; }

        public ElementReference NewProductNameBox;
        public ElementReference AddProductButton;
        public CartProductValidator cartProductValidator;
        public StoreProductValidator storeProductValidator;

        public async Task AddNewProduct()
        {
            cartProductValidator = new CartProductValidator(ViewModel.CartProducts);
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
                ModalViewModel.Message = message;
            }
        }

        public async Task SaveCartProduct(CartProductUIModel product)
        {
            ViewModel.CartProducts.Add(product);
            try
            {
                if (ViewModel.IsPolling)
                {
                    product.Id = await ViewModel.CartHub.InvokeAsync<int>(nameof(ICartHub.CartItemAdded), product);
                }
                else
                {
                    await CartProductsService.SaveCartProduct(product);
                }
            }
            catch (Exception ex)
            {
                ModalViewModel.Message = ex.Message;
            }
        }

        public Task MarkItemCollected(ChangeEventArgs e, CartProductUIModel product)
        {
            product.IsCollected = (bool)e.Value;
            ViewModel.OnPropertyChanged();
            if (ViewModel.IsPolling)
            {
                return ViewModel.CartHub.SendAsync(nameof(ICartHub.CartItemCollected), product.Id);
            }
            else
            {
                return CartProductsService.MarkCartProductCollected(product.Id);
            }
        }

        public Task SaveStoreProduct(string productName, double unitPrice)
        {
            StoreProductUIModel product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == productName);
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
                storeProductValidator = new StoreProductValidator(ViewModel.StoreProducts);
                product = new StoreProductUIModel() { Name = productName, UnitPrice = unitPrice };
                if (storeProductValidator.Validate(product).IsValid)
                {
                    ViewModel.StoreProducts.Add(product);
                    return StoreProductsService.SaveStoreProduct(product);
                }
                return Task.CompletedTask;
            }
        }

        public void StartEditItem(CartProductUIModel product)
        {
            EditingItem = product;
        }

        public async Task UpdateCartProduct(CartProductUIModel product)
        {
            cartProductValidator = new CartProductValidator(ViewModel.CartProducts);
            string message = string.Join(" ", cartProductValidator.Validate(product).Errors.Select(x => x.ErrorMessage));
            if (string.IsNullOrEmpty(message))
            {
                EditingItem = null;
                ViewModel.OnPropertyChanged();
                if (ViewModel.IsPolling)
                {
                    await ViewModel.CartHub.SendAsync(nameof(ICartHub.CartItemModified), product);
                }
                else
                {
                    await CartProductsService.UpdateCartProduct(product);
                }
            }
            ModalViewModel.Message = message;
        }

        public void CancelProductUpdate()
        {
            EditingItem = null;
        }

        public void GetItemPrice()
        {
            StoreProductUIModel product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == NewProduct.Name);
            if (product?.UnitPrice > 0)
            {
                NewProduct.UnitPrice = product.UnitPrice;
            }
        }

        public void MoveUp(CartProductUIModel cartProduct)
        {
            int index = ViewModel.CartProducts.IndexOf(cartProduct);
            if (index > 0)
            {
                ViewModel.CartProducts.Move(index, index - 1);
            }
        }

        public void MoveDown(CartProductUIModel cartProduct)
        {
            int index = ViewModel.CartProducts.IndexOf(cartProduct);
            if (index < ViewModel.CartProducts.Count-1)
            {
                ViewModel.CartProducts.Move(index, index + 1);
            }
        }

        public Task RemoveProduct(CartProductUIModel product)
        {
            ViewModel.CartProducts.Remove(product);
            if (ViewModel.IsPolling)
            {
                return ViewModel.CartHub.SendAsync(nameof(ICartHub.CartItemDeleted), product.Id);
            }
            else
            {
                return CartProductsService.DeleteCartProduct(product.Id);
            }
        }

        public CartProductUIModel dragTarget;
        public void DragStarted(CartProductUIModel product)
        {
            dragTarget = product;
        }

        public void OnDrop(CartProductUIModel product)
        {
            int dragTargetIndex = ViewModel.CartProducts.IndexOf(dragTarget);
            int dropTargetIndex = ViewModel.CartProducts.IndexOf(product);
            if (dragTargetIndex != dropTargetIndex)
            {
                ViewModel.CartProducts.Remove(dragTarget);
                ViewModel.CartProducts.Insert(dropTargetIndex, dragTarget);
            }
        }
    }
}

using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Client.ViewModels;
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
        public CartProductUIModel NewProduct { get; set; }
        public CartProductUIModel EditingItem { get; set; }
        public CartProductUIModel MovingItem { get; set; }
        public bool IsMoving { get; set; }

        public ElementReference NewProductNameBox;
        public ElementReference AddProductButton;
        public CartProductValidator cartProductValidator;
        public StoreProductValidator storeProductValidator;

        protected override void OnInitialized()
        {
            NewProduct = new CartProductUIModel() { Amount = 1 };
            base.OnInitialized();
        }

        public async Task AddNewProduct()
        {
            cartProductValidator = new CartProductValidator(ViewModel.CartProducts);
            string message = string.Join(" ", cartProductValidator.Validate(NewProduct).Errors.Select(x => x.ErrorMessage));
            if (string.IsNullOrEmpty(message))
            {
                CartProductUIModel newProduct = NewProduct;
                NewProduct = new CartProductUIModel() { Amount = 1 };
                await SaveCartProduct(newProduct);
                await SaveStoreProduct(newProduct.Name, newProduct.UnitPrice);
                await NewProductNameBox.FocusAsync();
            }
            else
            {
                ModalViewModel.Message = message;
            }
        }

        public async Task SaveCartProduct(CartProductUIModel product)
        {
            try
            {
                if (ViewModel.IsPolling)
                {
                    product.Id = await ViewModel.CartHub.InvokeAsync<int>(nameof(ICartHubActions.CartItemAdded), product);
                }
                else
                {
                    await CartProductsService.SaveCartProduct(product);
                }
                ViewModel.CartProducts.Add(product);
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
                return ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemCollected), product.Id);
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
                    await ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemModified), product);
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

        public async Task Move(CartProductUIModel cartProduct)
        {
            if (MovingItem == null)
            {
                MovingItem = cartProduct;
            }
            else
            {
                if (cartProduct != MovingItem)
                {
                    int itemIndex = ViewModel.CartProducts.IndexOf(MovingItem);
                    int newIndex = ViewModel.CartProducts.IndexOf(cartProduct);
                    ViewModel.CartProducts.Move(itemIndex, newIndex);
                    if (ViewModel.IsPolling)
                    {
                        await ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemMoved), cartProduct.Id, newIndex);
                    }
                }
                MovingItem = null;
            }
        }

        public Task RemoveProduct(CartProductUIModel product)
        {
            ViewModel.CartProducts.Remove(product);
            if (ViewModel.IsPolling)
            {
                return ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemDeleted), product.Id);
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

        public async Task OnDrop(CartProductUIModel product)
        {
            int dragTargetIndex = ViewModel.CartProducts.IndexOf(dragTarget);
            int dropTargetIndex = ViewModel.CartProducts.IndexOf(product);
            if (dragTargetIndex != dropTargetIndex)
            {
                ViewModel.CartProducts.Move(dragTargetIndex, dropTargetIndex);
                if (ViewModel.IsPolling)
                {
                    await ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemMoved), dragTarget.Id, dropTargetIndex);
                }
            }
        }
    }
}

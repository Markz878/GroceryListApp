using Blazored.LocalStorage;
using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Components;

public class CartComponentBase : BasePage<IndexViewModel>
{
    [Inject] public ModalViewModel ModalViewModel { get; set; }
    [Inject] public ILocalStorageService LocalStorage { get; set; } 
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    private ICartProductsService cartProductsService;
    private IStoreProductsService storeProductsService;
    protected CartProductUIModel newProduct;
    protected CartProductUIModel editingItem;
    protected CartProductUIModel movingItem;
    protected bool isAuthenticated;
    protected ElementReference NewProductNameBox;
    protected ElementReference AddProductButton;

    protected override async Task OnInitializedAsync()
    {
        newProduct = new CartProductUIModel() { Amount = 1 };
        AuthenticationState authenticationState = await AuthenticationStateTask;
        if (authenticationState.User?.Identity?.IsAuthenticated == true)
        {
            isAuthenticated = true;
            cartProductsService = new CartProductsApiService(HttpClientFactory);
            storeProductsService = new StoreProductsAPIService(HttpClientFactory);
        }
        else
        {
            cartProductsService = new CartProductsLocalService(LocalStorage);
            storeProductsService = new StoreProductsLocalService(LocalStorage);
        }
        ViewModel.CartProducts.Clear();
        foreach (CartProductUIModel item in await cartProductsService.GetCartProducts())
        {
            ViewModel.CartProducts.Add(item);
        }
        ViewModel.StoreProducts.Clear();
        foreach (StoreProductUIModel item in await storeProductsService.GetStoreProducts())
        {
            ViewModel.StoreProducts.Add(item);
        }
        base.OnInitialized();
    }

    public async Task AddNewProduct()
    {
        CartProductValidator cartProductValidator = new(ViewModel.CartProducts);
        string message = string.Join(" ", cartProductValidator.Validate(newProduct).Errors.Select(x => x.ErrorMessage));
        if (string.IsNullOrEmpty(message))
        {
            CartProductUIModel p = newProduct;
            newProduct = new CartProductUIModel() { Amount = 1 };
            await SaveCartProduct(p);
            await SaveStoreProduct(p.Name, p.UnitPrice);
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
                await cartProductsService.SaveCartProduct(product);
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
            return cartProductsService.MarkCartProductCollected(product.Id);
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
                return storeProductsService.UpdateStoreProductPrice(product.Id, unitPrice);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
        else
        {
            StoreProductValidator storeProductValidator = new(ViewModel.StoreProducts);
            product = new StoreProductUIModel() { Name = productName, UnitPrice = unitPrice };
            if (storeProductValidator.Validate(product).IsValid)
            {
                ViewModel.StoreProducts.Add(product);
                return storeProductsService.SaveStoreProduct(product);
            }
            return Task.CompletedTask;
        }
    }

    public void StartEditItem(CartProductUIModel product)
    {
        editingItem = product;
    }

    public async Task UpdateCartProduct(CartProductUIModel product)
    {
        CartProductValidator cartProductValidator = new(ViewModel.CartProducts);
        string message = string.Join(" ", cartProductValidator.Validate(product).Errors.Select(x => x.ErrorMessage));
        if (string.IsNullOrEmpty(message))
        {
            editingItem = null;
            ViewModel.OnPropertyChanged();
            if (ViewModel.IsPolling)
            {
                await ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemModified), product);
            }
            else
            {
                await cartProductsService.UpdateCartProduct(product);
            }
        }
        ModalViewModel.Message = message;
    }

    public void CancelProductUpdate()
    {
        editingItem = null;
    }

    public void GetItemPrice()
    {
        StoreProductUIModel product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == newProduct.Name);
        if (product?.UnitPrice > 0)
        {
            newProduct.UnitPrice = product.UnitPrice;
        }
    }

    public async Task Move(CartProductUIModel cartProduct)
    {
        if (movingItem == null)
        {
            movingItem = cartProduct;
        }
        else
        {
            if (cartProduct != movingItem)
            {
                int itemIndex = ViewModel.CartProducts.IndexOf(movingItem);
                int newIndex = ViewModel.CartProducts.IndexOf(cartProduct);
                ViewModel.CartProducts.Move(itemIndex, newIndex);
                if (ViewModel.IsPolling)
                {
                    await ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemMoved), cartProduct.Id, newIndex);
                }
            }
            movingItem = null;
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
            return cartProductsService.DeleteCartProduct(product.Id);
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

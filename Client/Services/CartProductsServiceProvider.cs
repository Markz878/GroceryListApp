using Blazored.LocalStorage;
using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace GroceryListHelper.Client.Services;

public class CartProductsServiceProvider : ICartProductsService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private bool isAuthenticated;
    private bool isInitialized;
    private ICartProductsService actingCartService;

    public CartProductsServiceProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
    {
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> ClearCartProducts()
    {
        await SelectProvider();
        return await actingCartService.ClearCartProducts();
    }

    public async Task<bool> DeleteCartProduct(string id)
    {
        await SelectProvider();
        return await actingCartService.DeleteCartProduct(id);
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        await SelectProvider();
        return await actingCartService.GetCartProducts();
    }

    public async Task<bool> MarkCartProductCollected(string id)
    {
        await SelectProvider();
        return await actingCartService.MarkCartProductCollected(id);
    }

    public async Task<bool> SaveCartProduct(CartProductUIModel product)
    {
        await SelectProvider();
        return await actingCartService.SaveCartProduct(product);
    }

    public async Task<bool> UpdateCartProduct(CartProductUIModel cartProduct)
    {
        await SelectProvider();
        return await actingCartService.UpdateCartProduct(cartProduct);
    }

    private async ValueTask SelectProvider()
    {
        if (!isInitialized || actingCartService is null)
        {
            isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
            actingCartService = isAuthenticated ? new CartProductsApiService(httpClientFactory) : new CartProductsLocalService(localStorage);
            isInitialized = true;
        }
    }
}

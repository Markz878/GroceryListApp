using Blazored.LocalStorage;
using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;

namespace GroceryListHelper.Client.Services;

public class CartProductsServiceProvider : ICartProductsService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly IndexViewModel viewModel;
    private bool isAuthenticated;
    private ICartProductsService actingCartService;

    public CartProductsServiceProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider, IndexViewModel viewModel)
    {
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.authenticationStateProvider = authenticationStateProvider;
        this.viewModel = viewModel;
    }

    public async Task DeleteAllCartProducts()
    {
        await SelectProvider();
        await actingCartService.DeleteAllCartProducts();
    }

    public async Task DeleteCartProduct(string id)
    {
        await SelectProvider();
        await actingCartService.DeleteCartProduct(id);
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        await SelectProvider();
        return await actingCartService.GetCartProducts();
    }

    public async Task SaveCartProduct(CartProductUIModel product)
    {
        await SelectProvider();
        await actingCartService.SaveCartProduct(product);
    }

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        await SelectProvider();
        await actingCartService.UpdateCartProduct(cartProduct);
    }

    private async ValueTask SelectProvider()
    {
        if (actingCartService is null)
        {
            isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
            if (isAuthenticated)
            {
                if (viewModel.IsPolling)
                {
                    actingCartService = new CartProductsSignalRService(viewModel);
                }
                else
                {
                    actingCartService = new CartProductsApiService(httpClientFactory);
                }
            }
            else
            {
                actingCartService = new CartProductsLocalService(localStorage);
            }
        }
    }
}

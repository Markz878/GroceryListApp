using Blazored.LocalStorage;
using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.StoreProduct;
using Microsoft.AspNetCore.Components.Authorization;

namespace GroceryListHelper.Client.Services;

public class StoreProductsServiceProvider : IStoreProductsService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private bool isAuthenticated;
    private bool isInitialized;
    private IStoreProductsService actingStoreService;

    public StoreProductsServiceProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
    {
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> ClearStoreProducts()
    {
        await SelectProvider();
        return await actingStoreService.ClearStoreProducts();
    }

    public async Task<List<StoreProductUIModel>> GetStoreProducts()
    {
        await SelectProvider();
        return await actingStoreService.GetStoreProducts();
    }

    public async Task<string> SaveStoreProduct(StoreProductModel product)
    {
        await SelectProvider();
        return await actingStoreService.SaveStoreProduct(product);
    }

    public async Task<bool> UpdateStoreProduct(StoreProductUIModel product)
    {
        await SelectProvider();
        return await actingStoreService.UpdateStoreProduct(product);
    }

    private async ValueTask SelectProvider()
    {
        if (!isInitialized || actingStoreService is null)
        {
            isAuthenticated = await authenticationStateProvider.IsUserAuthenticated();
            actingStoreService = isAuthenticated ? new StoreProductsAPIService(httpClientFactory) : new StoreProductsLocalService(localStorage);
            isInitialized = true;
        }
    }
}

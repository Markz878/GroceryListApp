using Blazored.LocalStorage;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.StoreProducts;
using Microsoft.AspNetCore.Components.Authorization;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Collections.ObjectModel;

namespace GroceryListHelper.Tests.Client;

public class StoreProductsServiceProviderTests
{
    private readonly IHttpClientFactory httpFactory = Substitute.For<IHttpClientFactory>();
    private readonly ILocalStorageService localStorage = Substitute.For<ILocalStorageService>();
    private readonly AuthenticationStateProvider authProvider = Substitute.For<AuthenticationStateProvider>();
    private readonly MainViewModel mainViewModel = new();
    private readonly IStoreProductsServiceFactory storeProductsServiceFactory;
    private readonly MockHttpMessageHandler _handlerMock = new();
    private const string storeProductsKey = "storeProducts";
    private const string baseUri = "https://localhost:5001";
    private const string fullUri = $"{baseUri}/api/storeproducts";

    public StoreProductsServiceProviderTests()
    {
        httpFactory.CreateClient("ProtectedClient").Returns(new HttpClient(_handlerMock) { BaseAddress = new Uri(baseUri) });
        IStoreProductsService[] storeProductsServices = new IStoreProductsService[]
        {
            new StoreProductsLocalService(localStorage, mainViewModel),
            new StoreProductsAPIService(httpFactory),
        };
        storeProductsServiceFactory = new StoreProductsServiceFactory(storeProductsServices, authProvider);
    }

    private void AuthProviderSetAnonymous()
    {
        authProvider.GetAuthenticationStateAsync().ReturnsForAnyArgs(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    private void AuthProviderSetAuthenticated()
    {
        authProvider.GetAuthenticationStateAsync().ReturnsForAnyArgs(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("name", "test") }, "Cookie", "name", "role")))));
    }

    [Fact]
    public async Task GetStoreProducts_WhenAnonymous_CallsLocalStorageGetItem()
    {
        AuthProviderSetAnonymous();
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.GetStoreProducts();
        await localStorage.Received(1).GetItemAsync<List<StoreProduct>>(storeProductsKey);
    }

    [Fact]
    public async Task GetStoreProducts_WhenAuthenticated_CallsApiGet()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Get, fullUri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StoreProduct>()));
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.GetStoreProducts();
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task SaveStoreProduct_WhenAnonymous_CallsLocalStorageSave()
    {
        AuthProviderSetAnonymous();
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.SaveStoreProduct(new StoreProduct());
        await localStorage.Received(1).SetItemAsync(storeProductsKey, Arg.Any<ObservableCollection<StoreProduct>>());
    }

    [Fact]
    public async Task SaveStoreProduct_WhenAuthenticated_CallsApiPost()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Post, fullUri)
            .Respond(HttpStatusCode.Created);
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.SaveStoreProduct(new StoreProduct());
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task ClearStoreProducts_WhenAnonymous_CallsLocalStorageClear()
    {
        AuthProviderSetAnonymous();
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.ClearStoreProducts();
        await localStorage.Received(1).RemoveItemAsync(storeProductsKey);
    }

    [Fact]
    public async Task ClearStoreProducts_WhenAuthenticated_CallsApiDelete()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Delete, fullUri)
            .Respond(HttpStatusCode.NoContent);
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.ClearStoreProducts();
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task UpdateStoreProduct_WhenAnonymous_CallsLocalSetItems()
    {
        AuthProviderSetAnonymous();
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.UpdateStoreProduct(new StoreProduct());
        await localStorage.Received(1).SetItemAsync(storeProductsKey, Arg.Any<ObservableCollection<StoreProduct>>());
    }

    [Fact]
    public async Task UpdateStoreProduct_WhenAuthenticated_CallsApiPut()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Put, fullUri)
            .Respond(HttpStatusCode.NoContent);
        IStoreProductsService storeProductsService = await storeProductsServiceFactory.GetStoreProductsService();
        await storeProductsService.UpdateStoreProduct(new StoreProduct());
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }
}

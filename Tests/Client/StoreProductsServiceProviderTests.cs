using Blazored.LocalStorage;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
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
    private readonly StoreProductsServiceProvider storeProductsService;
    private readonly MockHttpMessageHandler _handlerMock = new();
    private const string storeProductsKey = "storeProducts";
    private const string baseUri = "https://localhost:5001";
    private const string fullUri = $"{baseUri}/api/storeproducts";

    public StoreProductsServiceProviderTests()
    {
        storeProductsService = new(httpFactory, localStorage, authProvider, mainViewModel);
        httpFactory.CreateClient("ProtectedClient").Returns(new HttpClient(_handlerMock) { BaseAddress = new Uri(baseUri) });
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
        await storeProductsService.GetStoreProducts();
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task SaveStoreProduct_WhenAnonymous_CallsLocalStorageSave()
    {
        AuthProviderSetAnonymous();
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
        await storeProductsService.SaveStoreProduct(new StoreProduct());
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task ClearStoreProducts_WhenAnonymous_CallsLocalStorageClear()
    {
        AuthProviderSetAnonymous();
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
        await storeProductsService.ClearStoreProducts();
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task UpdateStoreProduct_WhenAnonymous_CallsLocalSetItems()
    {
        AuthProviderSetAnonymous();
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
        await storeProductsService.UpdateStoreProduct(new StoreProduct());
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }
}

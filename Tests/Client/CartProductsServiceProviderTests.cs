using Blazored.LocalStorage;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Collections.ObjectModel;

namespace GroceryListHelper.Tests.Client;

public class CartProductsServiceProviderTests
{
    private readonly IHttpClientFactory httpFactory = Substitute.For<IHttpClientFactory>();
    private readonly ILocalStorageService localStorage = Substitute.For<ILocalStorageService>();
    private readonly AuthenticationStateProvider authProvider = Substitute.For<AuthenticationStateProvider>();
    private readonly MainViewModel mainViewModel = new();
    private readonly ICartHubClient cartHubClient = Substitute.For<ICartHubClient>();
    private readonly NavigationManager navigation = new FakeNavigationManager("localhost:5001", "localhost:5001");
    private readonly CartProductsServiceProvider cartProductsService;
    private readonly MockHttpMessageHandler _handlerMock = new();
    private const string cartProductsKey = "cartProducts";
    private const string baseUri = "https://localhost:5001";
    private const string fullUri = $"{baseUri}/api/cartproducts";

    public CartProductsServiceProviderTests()
    {
        cartProductsService = new(httpFactory, localStorage, authProvider, cartHubClient, mainViewModel, navigation);
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
    public async Task GetCartProducts_WhenAnonymous_CallsLocalStorageGetItem()
    {
        AuthProviderSetAnonymous();
        await cartProductsService.GetCartProducts();
        await localStorage.Received(1).GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
    }

    [Fact]
    public async Task GetCartProducts_WhenAuthenticated_CallsApiGet()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Get, fullUri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(new List<CartProduct>()));
        await cartProductsService.GetCartProducts();
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task SaveCartProduct_WhenAnonymous_CallsLocalStorageSave()
    {
        AuthProviderSetAnonymous();
        await cartProductsService.SaveCartProduct(new CartProduct());
        await localStorage.Received(1).SetItemAsync(cartProductsKey, Arg.Any<ObservableCollection<CartProductUIModel>>());
    }

    [Fact]
    public async Task SaveCartProduct_WhenAuthenticated_CallsApiPost()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Post, fullUri)
            .Respond(HttpStatusCode.Created);
        await cartProductsService.SaveCartProduct(new CartProduct());
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task ClearCartProducts_WhenAnonymous_CallsLocalStorageClear()
    {
        AuthProviderSetAnonymous();
        await cartProductsService.DeleteAllCartProducts();
        await localStorage.Received(1).RemoveItemAsync(cartProductsKey);
    }

    [Fact]
    public async Task ClearCartProducts_WhenAuthenticated_CallsApiDelete()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Delete, fullUri)
            .Respond(HttpStatusCode.NoContent);
        await cartProductsService.DeleteAllCartProducts();
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task DeleteCartProduct_WhenAnonymous_CallsLocalStorageClear()
    {
        AuthProviderSetAnonymous();
        await cartProductsService.DeleteCartProduct("test");
        await localStorage.Received(1).SetItemAsync(cartProductsKey, Arg.Any<ObservableCollection<CartProductUIModel>>());
    }

    [Fact]
    public async Task DeleteCartProduct_WhenAuthenticated_CallsApiDelete()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Delete, $"{fullUri}/test")
            .Respond(HttpStatusCode.NoContent);
        await cartProductsService.DeleteCartProduct("test");
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task UpdateCartProduct_WhenAnonymous_CallsLocalSetItems()
    {
        AuthProviderSetAnonymous();
        await cartProductsService.UpdateCartProduct(new CartProductCollectable());
        await localStorage.Received(1).SetItemAsync(cartProductsKey, Arg.Any<ObservableCollection<CartProductUIModel>>());
    }

    [Fact]
    public async Task UpdateCartProduct_WhenAuthenticated_CallsApiPut()
    {
        AuthProviderSetAuthenticated();
        MockedRequest request = _handlerMock
            .When(HttpMethod.Put, fullUri)
            .Respond(HttpStatusCode.NoContent);
        await cartProductsService.UpdateCartProduct(new CartProductCollectable());
        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }
}

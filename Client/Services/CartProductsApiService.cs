using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared;
using System.Net.Http.Json;

namespace GroceryListHelper.Client.Services;

public class CartProductsApiService : ICartProductsService
{
    private readonly HttpClient client;
    private const string uri = "api/cartproducts";

    public CartProductsApiService(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("ProtectedClient");
    }

    public Task<List<CartProductUIModel>> GetCartProducts()
    {
        return client.GetFromJsonAsync<List<CartProductUIModel>>(uri) ?? Task.FromResult(new List<CartProductUIModel>());
    }

    public async Task SaveCartProduct(CartProductUIModel product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        if (response.IsSuccessStatusCode)
        {
            CartProductCollectable cartProduct = await response.Content.ReadFromJsonAsync<CartProductCollectable>();
            product.Id = cartProduct.Id;
        }
    }

    public async Task DeleteCartProduct(string id)
    {
        HttpResponseMessage response = await client.DeleteAsync(uri + $"/{id}");
    }

    public async Task DeleteAllCartProducts()
    {
        HttpResponseMessage response = await client.DeleteAsync(uri);
    }

    public async Task MarkCartProductCollected(string id)
    {
        HttpResponseMessage response = await client.PatchAsync(uri, null);
    }

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(uri, cartProduct);
    }
}

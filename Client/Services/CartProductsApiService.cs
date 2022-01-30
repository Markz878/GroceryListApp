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

    public async Task<bool> SaveCartProduct(CartProductUIModel product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        if (response.IsSuccessStatusCode)
        {
            CartProductCollectable cartProduct = await response.Content.ReadFromJsonAsync<CartProductCollectable>();
            product.Id = cartProduct.Id;
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteCartProduct(string id)
    {
        HttpResponseMessage response = await client.DeleteAsync(uri + $"/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ClearCartProducts()
    {
        HttpResponseMessage response = await client.DeleteAsync(uri);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> MarkCartProductCollected(string id)
    {
        HttpResponseMessage response = await client.PatchAsync(uri + $"/{id}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCartProduct(CartProductUIModel cartProduct)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(uri + $"/{cartProduct.Id}", cartProduct);
        return response.IsSuccessStatusCode;
    }
}

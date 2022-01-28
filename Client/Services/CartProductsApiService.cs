using GroceryListHelper.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
        string content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            product.Id = int.Parse(content);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteCartProduct(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync(uri + $"/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ClearCartProducts()
    {
        HttpResponseMessage response = await client.DeleteAsync(uri);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> MarkCartProductCollected(int id)
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

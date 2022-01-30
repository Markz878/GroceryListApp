using GroceryListHelper.Client.Models;
using System.Net.Http.Json;

namespace GroceryListHelper.Client.Services;

public class StoreProductsAPIService : IStoreProductsService
{
    private readonly HttpClient client;
    private const string uri = "api/storeproducts";

    public StoreProductsAPIService(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("ProtectedClient");
    }

    public async Task<List<StoreProductUIModel>> GetStoreProducts()
    {
        return await client.GetFromJsonAsync<List<StoreProductUIModel>>(uri) ?? new List<StoreProductUIModel>();
    }

    public async Task<bool> SaveStoreProduct(StoreProductUIModel product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        if (response.IsSuccessStatusCode)
        {
            product.Id = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }
        else
        {
            return response.IsSuccessStatusCode;
        }
    }

    public async Task<bool> ClearStoreProducts()
    {
        HttpResponseMessage response = await client.DeleteAsync(uri);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStoreProductPrice(string id, double price)
    {
        HttpResponseMessage response = await client.PatchAsync(uri + $"/{id}?price={price}", null);
        return response.IsSuccessStatusCode;
    }
}

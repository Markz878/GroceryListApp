using GroceryListHelper.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

    public async Task SaveStoreProduct(StoreProductUIModel product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        if (response.IsSuccessStatusCode)
        {
            string id = await response.Content.ReadAsStringAsync();
            product.Id = int.Parse(id);
        }
        else
        {
            throw new InvalidOperationException("Could not add product");
        }
    }

    public async Task ClearStoreProducts()
    {
        await client.DeleteAsync(uri);
    }

    public async Task UpdateStoreProductPrice(int id, double price)
    {
        await client.PatchAsync(uri + $"/{id}?price={price}", null);
    }
}

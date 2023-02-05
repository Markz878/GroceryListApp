namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsAPIService : IStoreProductsService
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

    public async Task<string> SaveStoreProduct(StoreProduct product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        if (response.IsSuccessStatusCode)
        {
            string id = await response.Content.ReadAsStringAsync();
            return id;
        }
        throw new Exception("Could not save store product.");
    }

    public async Task<bool> ClearStoreProducts()
    {
        HttpResponseMessage response = await client.DeleteAsync(uri);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStoreProduct(StoreProductUIModel storeProduct)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(uri, storeProduct);
        return response.IsSuccessStatusCode;
    }
}

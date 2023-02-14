namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsAPIService : IStoreProductsService
{
    private readonly HttpClient client;
    private const string uri = "api/storeproducts";

    public StoreProductsAPIService(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("ProtectedClient");
    }

    public async Task<List<StoreProduct>> GetStoreProducts()
    {
        return await client.GetFromJsonAsync<List<StoreProduct>>(uri) ?? new List<StoreProduct>();
    }

    public async Task SaveStoreProduct(StoreProduct product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        response.EnsureSuccessStatusCode();
    }

    public async Task ClearStoreProducts()
    {
        HttpResponseMessage response = await client.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStoreProduct(StoreProduct storeProduct)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(uri, storeProduct);
        response.EnsureSuccessStatusCode();
    }
}

namespace GroceryListHelper.Client.Services;

public sealed class CartProductsApiService : ICartProductsService
{
    private readonly HttpClient client;
    private const string uri = "api/cartproducts";

    public CartProductsApiService(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("ProtectedClient");
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await client.GetFromJsonAsync<List<CartProductUIModel>>(uri) ?? new List<CartProductUIModel>();
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCartProduct(string productName)
    {
        HttpResponseMessage response = await client.DeleteAsync(uri + $"/{productName}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllCartProducts()
    {
        await client.DeleteAsync(uri);
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        await client.PutAsJsonAsync(uri, cartProduct);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        await client.PatchAsync(uri + "/sort/" + (int)sortDirection, null);
    }
}

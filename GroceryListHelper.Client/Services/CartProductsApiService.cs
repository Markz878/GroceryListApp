namespace GroceryListHelper.Client.Services;

public sealed class CartProductsApiService(IHttpClientFactory clientFactory) : ICartProductsService
{
    private readonly HttpClient client = clientFactory.CreateClient("Client");
    private const string uri = "api/cartproducts";

    public async Task<List<CartProductCollectable>> GetCartProducts()
    {
        return await client.GetFromJsonAsync<List<CartProductCollectable>>(uri) ?? [];
    }

    public async Task CreateCartProduct(CartProduct product)
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

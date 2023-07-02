namespace GroceryListHelper.Client.Services;

public sealed class CartProductsGroupService : ICartProductsService
{
    private readonly HttpClient client;
    private readonly NavigationManager navigation;

    public CartProductsGroupService(IHttpClientFactory clientFactory, NavigationManager navigation)
    {
        client = clientFactory.CreateClient("ProtectedClient");
        this.navigation = navigation;
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await client.GetFromJsonAsync<List<CartProductUIModel>>(GetUrl()) ?? new List<CartProductUIModel>();
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(GetUrl(), product);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCartProduct(string productName)
    {
        HttpResponseMessage response = await client.DeleteAsync(GetUrl() + $"/{productName}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllCartProducts()
    {
        await client.DeleteAsync(GetUrl());
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        await client.PutAsJsonAsync(GetUrl(), cartProduct);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        await client.PatchAsync(GetUrl() + "/sort/" + (int)sortDirection, null);
    }

    private string GetUrl()
    {
        string groupId = navigation.Uri[navigation.Uri.LastIndexOf('/')..];
        string url = $"api/cartgroupproducts{groupId}";
        return url;
    }
}

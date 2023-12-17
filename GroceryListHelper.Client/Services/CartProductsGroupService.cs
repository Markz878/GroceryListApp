namespace GroceryListHelper.Client.Services;

public sealed class CartProductsGroupService : ICartProductsService
{
    private readonly HttpClient _httpClient;
    private readonly ICartHubClient _cartHubClient;
    private readonly NavigationManager _navigation;
    public CartProductsGroupService(IHttpClientFactory clientFactory, ICartHubClient cartHubClient, NavigationManager navigation)
    {
        _httpClient = clientFactory.CreateClient("Client");
        _httpClient.DefaultRequestHeaders.Add("ConnectionId", cartHubClient.GetConnectionId());
        _cartHubClient = cartHubClient;
        _navigation = navigation;
    }

    public Task<List<CartProductCollectable>> GetCartProducts()
    {
        throw new NotImplementedException();
    }

    public async Task CreateCartProduct(CartProduct product)
    {
        Guid groupId = GetGroupId();
        await _cartHubClient.JoinGroup(groupId);
        await _httpClient.PostAsJsonAsync(GetUrl(groupId), product);
    }

    public async Task DeleteCartProduct(string productName)
    {
        Guid groupId = GetGroupId();
        await _cartHubClient.JoinGroup(groupId);
        await _httpClient.DeleteAsync(GetUrl(groupId) + $"/{productName}");
    }

    public async Task DeleteAllCartProducts()
    {
        Guid groupId = GetGroupId();
        await _cartHubClient.JoinGroup(groupId);
        await _httpClient.DeleteAsync(GetUrl(groupId));
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        Guid groupId = GetGroupId();
        await _cartHubClient.JoinGroup(groupId);
        await _httpClient.PutAsJsonAsync(GetUrl(groupId), cartProduct);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        Guid groupId = GetGroupId();
        await _cartHubClient.JoinGroup(groupId);
        await _httpClient.PatchAsync($"{GetUrl(groupId)}/sort/{(int)sortDirection}", null);
    }

    private static string GetUrl(Guid groupId)
    {
        return "api/cartgroupproducts/" + groupId;
    }

    private Guid GetGroupId()
    {
        string groupId = _navigation.Uri[(_navigation.Uri.LastIndexOf('/') + 1)..];
        return Guid.Parse(groupId);
    }
}

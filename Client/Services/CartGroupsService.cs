using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.Client.Services;

public class CartGroupsService : ICartGroupsService
{
    private readonly HttpClient client;
    private const string uri = "api/cartgroups";

    public CartGroupsService(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("ProtectedClient");
    }

    public async Task<List<CartGroup>> GetCartGroups()
    {
        List<CartGroup>? cartGroups = await client.GetFromJsonAsync<List<CartGroup>>(uri);
        ArgumentNullException.ThrowIfNull(cartGroups);
        return cartGroups;
    }

    public async Task<CartGroup?> CreateCartGroup(CreateCartGroupRequest cartGroup)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, cartGroup);
        if (response.IsSuccessStatusCode is false)
        {
            return null;
        }
        string id = await response.Content.ReadAsStringAsync();
        id = id.Trim('"');
        return new CartGroup() { Id = Guid.Parse(id), Name = cartGroup.Name, OtherUsers = cartGroup.OtherUsers.ToHashSet() };
    }

    public async Task UpdateCartGroup(CartGroup cartGroup)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(uri, cartGroup);
        response.EnsureSuccessStatusCode();
    }

    public async Task LeaveCartGroup(Guid groupId)
    {
        HttpResponseMessage response = await client.DeleteAsync($"{uri}/{groupId}");
        response.EnsureSuccessStatusCode();
    }

    public Task JoinGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task LeaveGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }
}

using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

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


    public async Task<CartGroup?> GetCartGroup(Guid groupId)
    {
        CartGroup? cartGroup = await client.GetFromJsonAsync<CartGroup>($"{uri}/{groupId}");
        return cartGroup;
    }

    public async Task<Response<CartGroup, UserNotFoundException>> CreateCartGroup(CreateCartGroupRequest cartGroup)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, cartGroup);
        string body = await response.Content.ReadAsStringAsync();
        body = body.Trim('"');
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return new UserNotFoundException(body[..body.IndexOf(" ")]);
        }
        return new CartGroup() { Id = Guid.Parse(body), Name = cartGroup.Name, OtherUsers = cartGroup.OtherUsers.ToHashSet() };
    }

    public async Task UpdateCartGroup(UpdateCartGroupNameRequest cartGroup)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(uri, cartGroup);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCartGroup(Guid groupId)
    {
        HttpResponseMessage response = await client.DeleteAsync($"{uri}/{groupId}");
        response.EnsureSuccessStatusCode();
    }
}

using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;
public class CartGroupTests : BaseTest
{
    private readonly string _uri = "api/cartgroups";
    private readonly ICartGroupRepository groupRepository;

    public CartGroupTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
        groupRepository = _scope.ServiceProvider.GetRequiredService<ICartGroupRepository>();
    }

    [Fact]
    public async Task GetGroupsForUserTest()
    {
        Guid groupId = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { TestAuthHandler.UserEmail, "test@email.com" });
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task GetGroupsById()
    {
        Guid groupId = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { TestAuthHandler.UserEmail, "test@email.com" });
        CartGroup? cartGroup = await _client.GetFromJsonAsync<CartGroup>($"{_uri}/{groupId}", _jsonOptions);
        Assert.NotNull(cartGroup);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task CreateGroup()
    {
        CreateCartGroupRequest request = new() { Name = "Test", OtherUsers = new HashSet<string>() { "testi@user.com" } };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, request, _jsonOptions);
        string body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Guid id = Guid.Parse(body.Trim('"'));
        Assert.NotEqual(Guid.Empty, id);
        await groupRepository.DeleteCartGroup(id, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task DeleteGroup()
    {
        Guid groupId = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { TestAuthHandler.UserEmail, "test@email.com" });
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        CartGroup? group = await groupRepository.GetCartGroup(groupId, TestAuthHandler.UserEmail);
        Assert.Null(group);
    }

    [Fact]
    public async Task DeleteGroup_Forbidden()
    {
        Guid groupId = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { "test1@email.com", "test2@email.com" });
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await groupRepository.DeleteCartGroup(groupId, "test1@email.com");
    }


    [Fact]
    public async Task UpdateGroup()
    {
        Guid groupId = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { TestAuthHandler.UserEmail, "test@email.com" });
        CartGroup group = new() { Id = groupId, Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, group);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        CartGroup? updatedGroup = await groupRepository.GetCartGroup(groupId, TestAuthHandler.UserEmail);
        Assert.NotNull(updatedGroup);
        Assert.Equal(group.Name, updatedGroup.Name);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task UpdateGroup_Forbidden()
    {
        Guid groupId = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { "test1@email.com", "test2@email.com" });
        CartGroup group = new() { Id = groupId, Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, group);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await groupRepository.DeleteCartGroup(groupId, "test1@email.com");
    }
}

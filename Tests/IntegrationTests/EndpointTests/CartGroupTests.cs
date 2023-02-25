using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;
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
    public async Task GetGroupsForUser()
    {
        Guid groupId = await CreateNewGroup();
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task GetGroupById()
    {
        Guid groupId = await CreateNewGroup();
        CartGroup? cartGroup = await _client.GetFromJsonAsync<CartGroup>($"{_uri}/{groupId}", _jsonOptions);
        Assert.NotNull(cartGroup);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task GetGroupById_NotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{_uri}/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGroupById_Forbidden()
    {
        Guid groupId = await CreateNewGroup(true);
        HttpResponseMessage response = await _client.GetAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task GetCartGroupName()
    {
        Guid groupId = await CreateNewGroup();
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }

    [Fact]
    public async Task CreateGroup()
    {
        CreateCartGroupRequest request = new() { Name = "Test", OtherUsers = new HashSet<string>() { TestAuthHandler.RandomEmail1 } };
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
        Guid groupId = await CreateNewGroup();
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Response<CartGroup, ForbiddenError, NotFoundError> groupResponse = await groupRepository.GetCartGroup(groupId, TestAuthHandler.UserEmail);
        Assert.False(groupResponse.IsSuccess);
    }

    [Fact]
    public async Task DeleteGroup_Forbidden()
    {
        Guid groupId = await CreateNewGroup(true);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await groupRepository.DeleteCartGroup(groupId, "test1@email.com");
    }


    [Fact]
    public async Task UpdateGroup()
    {
        Guid groupId = await CreateNewGroup();
        UpdateCartGroupNameRequest group = new() { GroupId = groupId, Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, group);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Response<CartGroup, ForbiddenError, NotFoundError> updatedGroupResponse = await groupRepository.GetCartGroup(groupId, TestAuthHandler.UserEmail);
        CartGroup? updatedGroup = updatedGroupResponse.Match<CartGroup?>(x => x, e => null, e => null);
        Assert.NotNull(updatedGroup);
        Assert.Equal(group.Name, updatedGroup.Name);
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }



    [Fact]
    public async Task UpdateGroup_WheUserNotPartOfGroup_ReturnNotFound()
    {
        Guid groupId = await CreateNewGroup(true);
        UpdateCartGroupNameRequest group = new() { GroupId = groupId, Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, group);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await groupRepository.DeleteCartGroup(groupId, "test1@email.com");
    }

    [Fact]
    public async Task UpdateGroup_WhenGroupIdInvalid_ReturnNotFound()
    {
        UpdateCartGroupNameRequest group = new() { GroupId = Guid.NewGuid(), Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, group);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

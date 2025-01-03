using GroceryListHelper.Core.Domain.CartGroups;
using GroceryListHelper.Core.Domain.HelperModels;
using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Server.Models.CartGroups;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class CartGroupTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : BaseTest(factory, testOutputHelper)
{
    private readonly string _uri = "api/cartgroups";

    [Fact]
    public async Task GetGroupsForUser()
    {
        Guid groupId = await CreateNewGroup();
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
    }

    [Fact]
    public async Task GetGroupById()
    {
        Guid groupId = await CreateNewGroup();
        CartGroup? cartGroup = await _client.GetFromJsonAsync<CartGroup>($"{_uri}/{groupId}", _jsonOptions);
        Assert.NotNull(cartGroup);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
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
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
    }

    [Fact]
    public async Task GetCartGroupName()
    {
        Guid groupId = await CreateNewGroup();
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
    }

    [Fact]
    public async Task CreateGroup()
    {
        CreateCartGroupRequest request = new() { Name = "Test", OtherUsers = [TestAuthHandler.RandomEmail1] };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, request, _jsonOptions);
        string body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Guid groupId = Guid.Parse(body.Trim('"'));
        Assert.NotEqual(Guid.Empty, groupId);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
    }

    [Fact]
    public async Task DeleteGroup()
    {
        Guid groupId = await CreateNewGroup();
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_Forbidden()
    {
        Guid groupId = await CreateNewGroup(true);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = "test1@email.com" });
    }


    [Fact]
    public async Task UpdateGroup()
    {
        Guid groupId = await CreateNewGroup();
        UpdateCartGroupNameRequest group = new() { Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{_uri}/{groupId}", group);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Result<CartGroup, ForbiddenError, NotFoundError> updatedGroupResponse = await _mediator.Send(new GetCartGroupQuery() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
        CartGroup? updatedGroup = updatedGroupResponse.Map<CartGroup?>(x => x, e => null, e => null);
        Assert.NotNull(updatedGroup);
        Assert.Equal(group.Name, updatedGroup.Name);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail });
    }



    [Fact]
    public async Task UpdateGroup_WheUserNotPartOfGroup_ReturnNotFound()
    {
        Guid groupId = await CreateNewGroup(true);
        UpdateCartGroupNameRequest group = new() { Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{_uri}/{groupId}", group);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = "test1@email.com" });
    }

    [Fact]
    public async Task UpdateGroup_WhenGroupIdInvalid_ReturnNotFound()
    {
        UpdateCartGroupNameRequest group = new() { Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{_uri}/{Guid.NewGuid()}", group);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

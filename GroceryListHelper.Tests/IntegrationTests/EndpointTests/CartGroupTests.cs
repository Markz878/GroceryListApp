using GroceryListHelper.Core.Domain.CartGroups;
using GroceryListHelper.Core.Domain.HelperModels;
using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Server.Models.CartGroups;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class CartGroupTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : BaseTest(factory, testOutputHelper)
{
    private readonly string _uri = "api/cartgroups";

    [Fact]
    public async Task GetGroupsForUser()
    {
        Guid groupId = await CreateNewGroup();
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetGroupById()
    {
        Guid groupId = await CreateNewGroup();
        CartGroup? cartGroup = await _client.GetFromJsonAsync<CartGroup>($"{_uri}/{groupId}", _jsonOptions, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(cartGroup);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetGroupById_NotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{_uri}/{Guid.NewGuid()}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGroupById_Forbidden()
    {
        Guid groupId = await CreateNewGroup(true);
        HttpResponseMessage response = await _client.GetAsync($"{_uri}/{groupId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetCartGroupName()
    {
        Guid groupId = await CreateNewGroup();
        List<CartGroup>? cartGroups = await _client.GetFromJsonAsync<List<CartGroup>>(_uri, _jsonOptions, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(cartGroups);
        Assert.True(cartGroups.Count > 0);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CreateGroup()
    {
        CreateCartGroupRequest request = new() { Name = "Test", OtherUsers = [TestAuthHandler.RandomEmail1] };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, request, _jsonOptions, cancellationToken: TestContext.Current.CancellationToken);
        string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Guid groupId = Guid.Parse(body.Trim('"'));
        Assert.NotEqual(Guid.Empty, groupId);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DeleteGroup()
    {
        Guid groupId = await CreateNewGroup();
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        response = await _client.DeleteAsync($"{_uri}/{groupId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_Forbidden()
    {
        Guid groupId = await CreateNewGroup(true);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{groupId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = "test1@email.com" }, TestContext.Current.CancellationToken);
    }


    [Fact]
    public async Task UpdateGroup()
    {
        Guid groupId = await CreateNewGroup();
        UpdateCartGroupNameRequest group = new() { Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{_uri}/{groupId}", group, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Result<CartGroup, ForbiddenError, NotFoundError> updatedGroupResponse = await _mediator.Send(new GetCartGroupQuery() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
        CartGroup? updatedGroup = updatedGroupResponse.Map<CartGroup?>(x => x, e => null, e => null);
        Assert.NotNull(updatedGroup);
        Assert.Equal(group.Name, updatedGroup.Name);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = TestAuthHandler.UserEmail }, TestContext.Current.CancellationToken);
    }



    [Fact]
    public async Task UpdateGroup_WheUserNotPartOfGroup_ReturnNotFound()
    {
        Guid groupId = await CreateNewGroup(true);
        UpdateCartGroupNameRequest group = new() { Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{_uri}/{groupId}", group, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await _mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = "test1@email.com" }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task UpdateGroup_WhenGroupIdInvalid_ReturnNotFound()
    {
        UpdateCartGroupNameRequest group = new() { Name = "Updated" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{_uri}/{Guid.NewGuid()}", group, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class CartHubTests : BaseTest
{
    private readonly CartHubClient hub;
    private readonly AppState mainVM = new();
    private const string _uri = "api/cartgroupproducts/";
    public CartHubTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
        hub = new(mainVM, new Uri(factory.Server.BaseAddress, "carthub"), o => o.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler());
    }

    [Fact]
    public async Task JoinAndLeaveGroup_Success()
    {
        Guid groupId = await CreateNewGroup();
        await hub.JoinGroup(groupId);
        CartProduct product1 = new() { Name = "Test Prod 1", Amount = 3, Order = 7000, UnitPrice = 2.4 };
        CartProduct product2 = new() { Name = "Test Prod 2", Amount = 2, Order = 5000, UnitPrice = 1.8 };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri + groupId, product1);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        response = await _client.PostAsJsonAsync(_uri + groupId, product2);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        response = await _client.DeleteAsync(_uri + groupId + "/" + product1.Name);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        response = await _client.PutAsJsonAsync(_uri + groupId, product2 with { Amount = 4, Order = 6000, UnitPrice = 3 });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await Task.Delay(1000);
        Assert.Equal(1, mainVM.CartProducts?.Count);
        Assert.Equal(4, mainVM.CartProducts?.First().Amount);
        Assert.Equal(6000, mainVM.CartProducts?.First().Order);
        Assert.Equal(3, mainVM.CartProducts?.First().UnitPrice);
        await hub.LeaveGroup(groupId);
    }

    [Fact]
    public async Task JoinGroup_WhenNotMember_ErrorShown()
    {
        Guid groupId = await CreateNewGroup(true);
        await hub.JoinGroup(groupId);
        Assert.Equal("Error", mainVM.Header);
    }
}

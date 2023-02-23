using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.Hubs;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.BaseModels;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;
using Microsoft.AspNetCore.SignalR.Client;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;
public class CartHubTests : BaseTest, IAsyncLifetime
{
    private readonly HubConnection hub;
    private List<CartProductCollectable>? cartProducts;

    public CartHubTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
        string hubUri = $"{factory.Server.BaseAddress}carthub";
        hub = new HubConnectionBuilder().WithUrl(hubUri, o => o.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler()).WithAutomaticReconnect().Build();
        hub.On<string>(nameof(ICartHubNotifications.GetMessage), testOutputHelper.WriteLine);
        hub.On<List<CartProductCollectable>>(nameof(ICartHubNotifications.ReceiveCart), (cartProducts) =>
        {
            this.cartProducts = cartProducts;
        });
    }

    public async Task InitializeAsync()
    {
        await hub.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await hub.DisposeAsync();
    }

    [Fact]
    public async Task JoinAndLeaveGroup_Success()
    {
        ICartGroupRepository cartGroupRepository = _scope.ServiceProvider.GetRequiredService<ICartGroupRepository>();
        Guid groupId = await CreateNewGroup();
        await SaveCartProducts(5, groupId);

        HubResponse joinResponse = await hub.InvokeAsync<HubResponse>(nameof(CartHub.JoinGroup), groupId);
        await Task.Delay(1000);
        Assert.Contains("You have joined cart", joinResponse.SuccessMessage);
        Assert.Equal("", joinResponse.ErrorMessage);
        Assert.Equal(5, cartProducts?.Count);

        Guid? userShouldBeInGroupId = await cartGroupRepository.GetUserCurrentShareGroup(TestAuthHandler.UserId);
        Assert.NotNull(userShouldBeInGroupId);
        Assert.Equal(groupId, userShouldBeInGroupId);

        HubResponse leaveResponse = await hub.InvokeAsync<HubResponse>(nameof(CartHub.LeaveGroup), groupId);
        Assert.Contains("You left", leaveResponse.SuccessMessage);

        Guid? userShouldNotBeInGroupId = await cartGroupRepository.GetUserCurrentShareGroup(TestAuthHandler.UserId);
        Assert.Null(userShouldNotBeInGroupId);
    }

    [Fact]
    public async Task JoinGroup_NoGroupWithId_ReturnError()
    {
        HubResponse response = await hub.InvokeAsync<HubResponse>(nameof(CartHub.JoinGroup), Guid.NewGuid());
        Assert.Equal("", response.SuccessMessage);
        Assert.Equal("User is not part of a group with given the id.", response.ErrorMessage);
    }
}

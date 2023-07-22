using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Core.RepositoryContracts;
using GroceryListHelper.Shared.Models.BaseModels;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;
public class CartHubTests : BaseTest, IAsyncLifetime
{
    private readonly CartHubClient hub;
    private readonly MainViewModel mainVM = new();
    private readonly ModalViewModel modalVM = new();

    public CartHubTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
        hub = new(new Uri(factory.Server.BaseAddress, "carthub"), mainVM, modalVM, o => o.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler());
    }

    public async Task InitializeAsync()
    {
        await hub.Start();
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

        HubResponse joinResponse = await hub.JoinGroup(groupId);
        await Task.Delay(1000);
        Assert.Equal("", joinResponse.ErrorMessage);
        Assert.Equal(5, mainVM.CartProducts?.Count);

        Guid? userShouldBeInGroupId = await cartGroupRepository.GetUserCurrentShareGroup(TestAuthHandler.UserId);
        Assert.NotNull(userShouldBeInGroupId);
        Assert.Equal(groupId, userShouldBeInGroupId);

        HubResponse addItemResponse = await hub.CartItemAdded(new CartProduct() { Name = "Test Prod", Amount = 3, Order = 7000, UnitPrice = 2.4 });
        Assert.Empty(addItemResponse.ErrorMessage);
        HubResponse modifyItemResponse = await hub.CartItemModified(new CartProductCollectable() { Name = "Test Prod", Amount = 4, Order = 7000, UnitPrice = 2.5 });
        Assert.Empty(modifyItemResponse.ErrorMessage);
        HubResponse deleteItemResponse = await hub.CartItemDeleted("Test Prod");
        Assert.Empty(deleteItemResponse.ErrorMessage);

        HubResponse leaveResponse = await hub.LeaveGroup(groupId);
        Assert.Empty(leaveResponse.ErrorMessage);

        Guid? userShouldNotBeInGroupId = await cartGroupRepository.GetUserCurrentShareGroup(TestAuthHandler.UserId);
        Assert.Null(userShouldNotBeInGroupId);
    }

    [Fact]
    public async Task JoinGroup_NoGroupWithId_ReturnError()
    {
        HubResponse response = await hub.JoinGroup(Guid.NewGuid());
        Assert.Equal("User is not part of the group with given the id.", response.ErrorMessage);
    }
}

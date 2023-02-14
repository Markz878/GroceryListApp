namespace GroceryListHelper.Server.Services;

public sealed class ServerCartHubClient : ICartHubClient
{
    public Task<HubResponse> CartItemAdded(CartProduct product)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> CartItemDeleted(string name)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> CartItemModified(CartProductCollectable product)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> JoinGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> LeaveGroup()
    {
        throw new NotImplementedException();
    }

    public ValueTask Start()
    {
        throw new NotImplementedException();
    }

    public ValueTask Stop()
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<HubResponse> LeaveGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }
}

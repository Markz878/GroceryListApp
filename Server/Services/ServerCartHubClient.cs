namespace GroceryListHelper.Server.Services;

public sealed class ServerCartHubClient : ICartHubClient
{
    public void BuildCartHubConnection()
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> CartItemAdded(CartProduct product)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> CartItemDeleted(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> CartItemModified(CartProductCollectable product)
    {
        throw new NotImplementedException();
    }

    public Task<HubResponse> CreateGroup(List<string> allowedUserEmails)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<HubResponse> JoinGroup(string cartHostEmail)
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
}

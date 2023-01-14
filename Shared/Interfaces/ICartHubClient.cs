namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubClient : ICartHubClientActions, IAsyncDisposable
{
    ValueTask Start();
    ValueTask Stop();
}
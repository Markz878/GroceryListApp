namespace GroceryListHelper.Shared.Interfaces;

public interface ICartHubBuilder : IDisposable
{
    void BuildCartHubConnection();
}
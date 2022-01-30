namespace GroceryListHelper.Server.Hubs;

public interface ICartHubService
{
    IDictionary<string, List<string>> GroupAllowedEmails { get; }
}

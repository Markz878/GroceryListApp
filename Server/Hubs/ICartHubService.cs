using System.Collections.Generic;

namespace GroceryListHelper.Server.Hubs
{
    public interface ICartHubService
    {
        IDictionary<int, List<string>> GroupAllowedEmails { get; }
    }
}
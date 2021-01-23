using System.Collections.Generic;

namespace GroceryListHelper.Server.Hubs
{
    public class CartHubService
    {
        /// <summary>
        /// CartHub groups' allowed emails. Key is the host user id and value is the other allowed emails that can join (set by the host).
        /// </summary>
        public IDictionary<int, string[]> GroupAllowedEmails { get; } = new Dictionary<int, string[]>();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Shared.Interfaces
{
    public interface ICartHub
    {
        Task<HubResponse> CreateGroup(List<string> allowedUsers);
        Task<HubResponse> JoinGroup(string hostEmail);
        Task<HubResponse> LeaveGroup();
        Task<int> CartItemAdded(CartProductCollectable product);
        Task CartItemModified(CartProductCollectable product);
        Task CartItemCollected(int id);
        Task CartItemDeleted(int id);
        Task CartItemMoved(int id, int newIndex);
    }
}
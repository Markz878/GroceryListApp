using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Shared.Interfaces
{
    public interface ICartHub
    {
        Task<HubResponse> CreateGroup(List<string> allowedUsers);
        Task<HubResponse> JoinGroup(string hostEmail);
        Task<HubResponse> LeaveGroup();
        Task<bool> CartItemAdded(CartProductCollectable product);
        Task<bool> CartItemModified(CartProductCollectable product);
        Task<bool> CartItemCollected(int id);
        Task<bool> CartItemDeleted(int id);
    }
}
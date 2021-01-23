using System.Threading.Tasks;

namespace GroceryListHelper.Shared.Interfaces
{
    public interface ICartHub
    {
        Task<bool> CartItemAdded(string hostEmail, CartProductCollectable product);
        Task<bool> CartItemCollected(int id);
        Task<bool> CartItemDeleted(int id);
        Task<bool> CartItemModified(string hostEmail, CartProductCollectable product);
        Task<HubResponse> CreateGroup(string[] allowedUsers);
        Task<HubResponse> JoinGroup(string hostEmail);
        Task<HubResponse> LeaveGroup(string hostEmail);
    }
}
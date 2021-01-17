using System.Threading.Tasks;

namespace GroceryListHelper.Shared.Interfaces
{
    public interface ICartHub
    {
        Task<bool> CartItemAdded(string hostEmail, CartProductCollectable product);
        Task<bool> CartItemCollected(int id);
        Task<bool> CartItemDeleted(int id);
        Task<bool> CartItemModified(string hostEmail, CartProductCollectable product);
        Task<string> CreateGroup(string[] allowedUsers);
        Task<string> JoinGroup(string hostEmail);
        Task<string> LeaveGroup(string hostEmail);
    }
}
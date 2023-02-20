using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartGroupRepository
{
    Task<List<CartGroup>> GetCartGroupsForUser(string userEmail);
    Task<CartGroup?> GetCartGroup(Guid groupId, string userEmail);
    Task<bool> CheckGroupAccess(Guid groupId, string userEmail);
    Task<Guid> CreateGroup(string name, HashSet<string> userEmails);
    Task<Exception?> DeleteCartGroup(Guid groupId, string userEmail);
    Task UserJoinedSharing(Guid userId, Guid groupId);
    Task UserLeftSharing(Guid userId, Guid groupId);
    Task<Guid?> GetUserCurrentShareGroup(Guid userId);
    Task<Exception?> UpdateGroupName(Guid groupId, string newName, string userEmail);
}

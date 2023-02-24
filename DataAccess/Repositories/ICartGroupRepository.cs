using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartGroupRepository
{
    Task<List<CartGroup>> GetCartGroupsForUser(string userEmail);
    Task<Response<CartGroup, Forbidden, NotFound>> GetCartGroup(Guid groupId, string userEmail);
    Task<bool> CheckGroupAccess(Guid groupId, string userEmail);
    Task<Response<Guid, NotFound>> CreateGroup(string name, HashSet<string> userEmails);
    Task<Response<string, NotFound>> GetCartGroupName(Guid groupId, string userEmail);
    Task<NotFound?> DeleteCartGroup(Guid groupId, string userEmail);
    Task UserJoinedSharing(Guid userId, Guid groupId);
    Task UserLeftSharing(Guid userId, Guid groupId);
    Task<Guid?> GetUserCurrentShareGroup(Guid userId);
    Task<NotFound?> UpdateGroupName(Guid groupId, string newName, string userEmail);
}

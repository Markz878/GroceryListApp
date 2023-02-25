using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartGroupRepository
{
    Task<List<CartGroup>> GetCartGroupsForUser(string userEmail);
    Task<Response<CartGroup, ForbiddenError, NotFoundError>> GetCartGroup(Guid groupId, string userEmail);
    Task<bool> CheckGroupAccess(Guid groupId, string userEmail);
    Task<Response<Guid, NotFoundError>> CreateGroup(string name, HashSet<string> userEmails);
    Task<Response<string, NotFoundError>> GetCartGroupName(Guid groupId, string userEmail);
    Task<NotFoundError?> DeleteCartGroup(Guid groupId, string userEmail);
    Task UserJoinedSharing(Guid userId, Guid groupId);
    Task UserLeftSharing(Guid userId, Guid groupId);
    Task<Guid?> GetUserCurrentShareGroup(Guid userId);
    Task<NotFoundError?> UpdateGroupName(Guid groupId, string newName, string userEmail);
}

using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartGroupRepository
{
    Task<List<CartGroup>> GetCartGroupsForUser(string userEmail);
    Task<Result<CartGroup, ForbiddenError, NotFoundError>> GetCartGroup(Guid groupId, string userEmail);
    Task<bool> CheckGroupAccess(Guid groupId, string userEmail);
    Task<Result<Guid, NotFoundError>> CreateGroup(string name, HashSet<string> userEmails);
    Task<Result<string, NotFoundError>> GetCartGroupName(Guid groupId, string userEmail);
    Task<NotFoundError?> DeleteCartGroup(Guid groupId, string userEmail);
    Task<ConflictError?> UserJoinedSharing(Guid userId, Guid groupId);
    Task<NotFoundError?> UserLeftSharing(Guid userId, Guid groupId);
    Task<Guid?> GetUserCurrentShareGroup(Guid userId);
    Task<NotFoundError?> UpdateGroupName(Guid groupId, string newName, string userEmail);
}

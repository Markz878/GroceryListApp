using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartGroupRepository
{
    Task<List<CartGroup>> GetCartGroupsForUser(string userEmail);
    Task<Response<CartGroup, ForbiddenException, NotFoundException>> GetCartGroup(Guid groupId, string userEmail);
    Task<bool> CheckGroupAccess(Guid groupId, string userEmail);
    Task<Guid> CreateGroup(string name, HashSet<string> userEmails);
    Task<Response<string, NotFoundException>> GetCartGroupName(Guid groupId, string userEmail);
    Task<NotFoundException?> DeleteCartGroup(Guid groupId, string userEmail);
    Task UserJoinedSharing(Guid userId, Guid groupId);
    Task UserLeftSharing(Guid userId, Guid groupId);
    Task<Guid?> GetUserCurrentShareGroup(Guid userId);
    Task<NotFoundException?> UpdateGroupName(Guid groupId, string newName, string userEmail);
}

using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.DataAccess.Repositories;

public interface ICartGroupRepository
{
    Task<List<CartGroup>> GetCartGroupsForUser(string userEmail);
    Task<Response<CartGroup, ForbiddenException, NotFoundException>> GetCartGroup(Guid groupId, string userEmail);
    Task<bool> CheckGroupAccess(Guid groupId, string userEmail);
    Task<Response<Guid, NotFoundException>> CreateGroup(string name, HashSet<string> userEmails);
    Task<Response<string, NotFoundException>> GetCartGroupName(Guid groupId, string userEmail);
    Task<NotFoundException?> DeleteCartGroup(Guid groupId, string userEmail);
    Task UserJoinedSharing(Guid userId, Guid groupId);
    Task UserLeftSharing(Guid userId, Guid groupId);
    Task<Guid?> GetUserCurrentShareGroup(Guid userId);
    Task<NotFoundException?> UpdateGroupName(Guid groupId, string newName, string userEmail);
}

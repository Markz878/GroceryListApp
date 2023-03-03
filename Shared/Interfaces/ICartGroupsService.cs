using GroceryListHelper.Shared.Models.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartGroupsService
{
    Task<Result<CartGroup, UserNotFoundException>> CreateCartGroup(CreateCartGroupRequest cartGroup);
    Task<List<CartGroup>> GetCartGroups();
    Task<CartGroup?> GetCartGroup(Guid groupId);
    Task DeleteCartGroup(Guid groupId);
    Task UpdateCartGroup(UpdateCartGroupNameRequest cartGroup);
}
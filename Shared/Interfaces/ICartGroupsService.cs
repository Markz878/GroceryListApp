using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.Shared.Interfaces;

public interface ICartGroupsService
{
    Task<CartGroup?> CreateCartGroup(CreateCartGroupRequest cartGroup);
    Task<List<CartGroup>> GetCartGroups();
    Task<CartGroup?> GetCartGroup(Guid groupId);
    Task DeleteCartGroup(Guid groupId);
    Task UpdateCartGroup(UpdateCartGroupNameRequest cartGroup);
}
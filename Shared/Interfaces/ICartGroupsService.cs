using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.Shared.Interfaces;
public interface ICartGroupsService
{
    Task<CartGroup?> CreateCartGroup(CreateCartGroupRequest cartGroup);
    Task<List<CartGroup>> GetCartGroups();
    Task LeaveCartGroup(Guid groupId);
    Task UpdateCartGroup(CartGroup cartGroup);
    Task JoinGroup(Guid groupId);
    Task LeaveGroup(Guid groupId);
}
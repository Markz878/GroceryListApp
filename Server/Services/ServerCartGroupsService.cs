using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Server.Services;

public class ServerCartGroupsService : ICartGroupsService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICartGroupRepository cartGroupRepository;

    public ServerCartGroupsService(IHttpContextAccessor httpContextAccessor, ICartGroupRepository cartGroupRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.cartGroupRepository = cartGroupRepository;
    }

    public async Task<CartGroup?> GetCartGroup(Guid groupId)
    {
        string? userEmail = httpContextAccessor.HttpContext?.User.GetDisplayName();
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return null;
        }
        Response<CartGroup, Forbidden, NotFound> groupResponse = await cartGroupRepository.GetCartGroup(groupId, userEmail);
        return groupResponse.Match<CartGroup?>(x => x, e => null, e => null);
    }

    public async Task<List<CartGroup>> GetCartGroups()
    {
        List<CartGroup> groups = await cartGroupRepository.GetCartGroupsForUser(httpContextAccessor.HttpContext!.User.GetUserEmail());
        return groups;
    }

    public Task DeleteCartGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCartGroup(UpdateCartGroupNameRequest cartGroup)
    {
        throw new NotImplementedException();
    }

    public Task<Response<CartGroup, UserNotFoundException>> CreateCartGroup(CreateCartGroupRequest cartGroup)
    {
        throw new NotImplementedException();
    }
}

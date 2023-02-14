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
    public Task<CartGroup?> CreateCartGroup(CreateCartGroupRequest cartGroup)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CartGroup>> GetCartGroups()
    {
        List<CartGroup> groups = await cartGroupRepository.GetCartGroupsForUser(httpContextAccessor.HttpContext?.User.GetUserEmail()!);
        return groups;
    }

    public Task JoinGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task LeaveCartGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task LeaveGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCartGroup(CartGroup cartGroup)
    {
        throw new NotImplementedException();
    }
}

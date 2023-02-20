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

    public async Task<CartGroup?> GetCartGroup(Guid groupId)
    {
        string? userEmail = httpContextAccessor.HttpContext?.User.GetUserEmail();
        if(string.IsNullOrWhiteSpace(userEmail))
        {
            return null;
        }
        CartGroup? group = await cartGroupRepository.GetCartGroup(groupId, userEmail);
        return group;
    }

    public async Task<List<CartGroup>> GetCartGroups()
    {
        List<CartGroup> groups = await cartGroupRepository.GetCartGroupsForUser(httpContextAccessor.HttpContext?.User.GetUserEmail()!);
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
}

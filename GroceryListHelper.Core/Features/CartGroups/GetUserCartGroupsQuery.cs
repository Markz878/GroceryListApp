namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record GetUserCartGroupsQuery : IRequest<List<CartGroup>>
{
    public required string UserEmail { get; init; }
}

internal sealed class GetCartGroupsQueryHandler(TableServiceClient db) : IRequestHandler<GetUserCartGroupsQuery, List<CartGroup>>
{
    public async Task<List<CartGroup>> Handle(GetUserCartGroupsQuery request, CancellationToken cancellationToken = default)
    {
        List<CartGroup> result = [];
        List<CartUserGroupDbModel> cartUserGroups = await db.GetTableEntitiesByPrimaryKey<CartUserGroupDbModel>(request.UserEmail, select: GetColumns());
        foreach (CartUserGroupDbModel? cartUserGroup in cartUserGroups.DistinctBy(x => x.GroupId))
        {
            List<CartGroupUserDbModel> cartGroupUsers = await db.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(cartUserGroup.GroupId.ToString());
            if (cartGroupUsers.Count > 1)
            {
                CartGroup group = new() { Id = cartGroupUsers[0].GroupId, Name = cartGroupUsers[0].Name };
                foreach (CartGroupUserDbModel? cartGroupUser in cartGroupUsers.Where(x => x.MemberEmail != request.UserEmail))
                {
                    group.OtherUsers.Add(cartGroupUser.MemberEmail);
                }
                result.Add(group);
            }
        }
        return result;
    }

    private static IEnumerable<string> GetColumns()
    {
        yield return "GroupId";
    }
}




namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record GetCartGroupQuery : IRequest<Result<CartGroup, ForbiddenError, NotFoundError>>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
}

internal sealed class GetCartGroupQueryHandler(TableServiceClient db) : IRequestHandler<GetCartGroupQuery, Result<CartGroup, ForbiddenError, NotFoundError>>
{
    public async Task<Result<CartGroup, ForbiddenError, NotFoundError>> Handle(GetCartGroupQuery request, CancellationToken cancellationToken = default)
    {
        List<CartGroupUserDbModel> cartGroupUsers = await db.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(request.GroupId.ToString());
        if (cartGroupUsers.Count == 0)
        {
            return new NotFoundError();
        }
        if (!cartGroupUsers.Any(x => x.MemberEmail == request.UserEmail))
        {
            return new ForbiddenError();
        }
        CartGroup group = new() { Id = request.GroupId, Name = cartGroupUsers[0].Name };
        foreach (CartGroupUserDbModel? member in cartGroupUsers.Where(x => x.MemberEmail != request.UserEmail))
        {
            group.OtherUsers.Add(member.MemberEmail);
        }
        return group;
    }
}


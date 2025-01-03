namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record GetCartGroupQuery : IRequest<Result<CartGroup, ForbiddenError, NotFoundError>>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
}

internal sealed class GetCartGroupQueryHandler(CosmosClient db) : IRequestHandler<GetCartGroupQuery, Result<CartGroup, ForbiddenError, NotFoundError>>
{
    public async Task<Result<CartGroup, ForbiddenError, NotFoundError>> Handle(GetCartGroupQuery request, CancellationToken cancellationToken = default)
    {
        Container groupsContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartGroupsContainer);
        CartGroupEntity cartGroupEntity = await groupsContainer.ReadItemAsync<CartGroupEntity>(request.GroupId.ToString(), new PartitionKey(request.GroupId.ToString()));

        if (!cartGroupEntity.MemberEmails.Any(x => x == request.UserEmail))
        {
            return new ForbiddenError();
        }
        CartGroup group = new() { Id = cartGroupEntity.Id, Name = cartGroupEntity.Name, OtherUsers = cartGroupEntity.MemberEmails};
        group.OtherUsers.Remove(request.UserEmail);
        return group;
    }
}


namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record UpdateGroupNameCommand : IRequest<NotFoundError?>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
    public required string GroupName { get; init; }
}

internal sealed class UpdateGroupNameCommandHandler(CosmosClient db) : IRequestHandler<UpdateGroupNameCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(UpdateGroupNameCommand request, CancellationToken cancellationToken = default)
    {
        bool hasAccess = await Common.CheckGroupAccess(db, request.GroupId, request.UserEmail, cancellationToken);
        if (hasAccess is false)
        {
            return new NotFoundError();
        }
        Container groupsContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartGroupsContainer);
        string id = request.GroupId.ToString();
        await groupsContainer.PatchItemAsync<CartGroupEntity>(id, new PartitionKey(id),
            [PatchOperation.Replace("/name", request.GroupName)]);
        return null;
    }
}


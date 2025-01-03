namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record DeleteCartGroupCommand : IRequest<NotFoundError?>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
}


internal sealed class DeleteCartGroupCommandHandler(CosmosClient db) : IRequestHandler<DeleteCartGroupCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(DeleteCartGroupCommand request, CancellationToken cancellationToken = default)
    {
        bool hasAccess = await Common.CheckGroupAccess(db, request.GroupId, request.UserEmail, cancellationToken);
        if (hasAccess is false)
        {
            return new NotFoundError();
        }
        await DeleteCartGroupProducts(db, request.GroupId);
        await DeleteCartUserGroups(db, request.GroupId);
        return null;
    }

    private static async Task DeleteCartUserGroups(CosmosClient db, Guid groupId)
    {
        Container groupsContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartGroupsContainer);
        await groupsContainer.DeleteItemStreamAsync(groupId.ToString(), new PartitionKey(groupId.ToString()));
    }

    private static async Task DeleteCartGroupProducts(CosmosClient db, Guid groupId)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartProductsContainer);
        await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(groupId.ToString()));
    }
}


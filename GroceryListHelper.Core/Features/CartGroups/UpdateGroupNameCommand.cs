namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record UpdateGroupNameCommand : IRequest<NotFoundError?>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
    public required string GroupName { get; init; }
}

internal sealed class UpdateGroupNameCommandHandler(TableServiceClient db) : IRequestHandler<UpdateGroupNameCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(UpdateGroupNameCommand request, CancellationToken cancellationToken = default)
    {
        bool hasAccess = await Common.CheckGroupAccess(db, request.GroupId, request.UserEmail, cancellationToken);
        if (hasAccess is false)
        {
            return new NotFoundError();
        }
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(request.GroupId.ToString());
        foreach (CartGroupUserDbModel cartGroupUser in cartGroupUsers)
        {
            cartGroupUser.Name = request.GroupName;
        }
        await cartGroupUserTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)), cancellationToken);
        return null;
    }
}


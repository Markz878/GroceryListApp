namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record DeleteCartGroupCommand : IRequest<NotFoundError?>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
}


internal sealed class DeleteCartGroupCommandHandler(TableServiceClient db) : IRequestHandler<DeleteCartGroupCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(DeleteCartGroupCommand request, CancellationToken cancellationToken = default)
    {
        bool hasAccess = await Common.CheckGroupAccess(db, request.GroupId, request.UserEmail, cancellationToken);
        if (hasAccess is false)
        {
            return new NotFoundError();
        }
        List<CartGroupUserDbModel> cartGroupUsers = await DeleteCartGroupUsers(db, request.GroupId, cancellationToken);
        await DeleteCartUserGroups(db, cartGroupUsers, cancellationToken);
        await DeleteCartGroupProducts(db, request.GroupId, cancellationToken);
        return null;
    }

    private static async Task<List<CartGroupUserDbModel>> DeleteCartGroupUsers(TableServiceClient db, Guid groupId, CancellationToken cancellationToken)
    {
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        List<CartGroupUserDbModel> cartGroupUsers = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
        await cartGroupUserTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)), cancellationToken);
        return cartGroupUsers;
    }

    private static async Task DeleteCartUserGroups(TableServiceClient db, List<CartGroupUserDbModel> cartGroupUsers, CancellationToken cancellationToken)
    {
        TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        await cartUserGroupTableClient.SubmitTransactionAsync(cartGroupUsers.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, new CartUserGroupDbModel() { MemberEmail = x.MemberEmail, GroupId = x.GroupId })), cancellationToken);
    }

    private static async Task DeleteCartGroupProducts(TableServiceClient db, Guid groupId, CancellationToken cancellationToken)
    {
        TableClient cartProductsTableClient = db.GetTableClient(CartProductDbModel.GetTableName());
        List<CartProductDbModel> cartProducts = await cartProductsTableClient.GetTableEntitiesByPrimaryKey<CartProductDbModel>(groupId.ToString());
        if (cartProducts.Count > 0)
        {
            await cartProductsTableClient.SubmitTransactionAsync(cartProducts.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)), cancellationToken);
        }
    }
}


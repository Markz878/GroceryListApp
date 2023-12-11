namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record DeleteAllUserStoreProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
}

internal sealed class DeleteAllUserStoreProductsCommandHandler(TableServiceClient db) : IRequestHandler<DeleteAllUserStoreProductsCommand>
{
    public async Task Handle(DeleteAllUserStoreProductsCommand request, CancellationToken cancellationToken = default)
    {
        List<StoreProductDbModel> products = await db.GetTableClient(StoreProductDbModel.GetTableName())
            .GetTableEntitiesByPrimaryKey<StoreProductDbModel>(request.UserId.ToString());
        if (products.Count > 0)
        {
            await db.GetTableClient(StoreProductDbModel.GetTableName())
                .SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)), cancellationToken);
        }
    }
}


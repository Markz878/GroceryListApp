namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record ClearCartProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
}

internal sealed class ClearCartProductsCommandHandler(TableServiceClient db) : IRequestHandler<ClearCartProductsCommand>
{
    public async Task Handle(ClearCartProductsCommand request, CancellationToken cancellationToken = default)
    {
        List<CartProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<CartProductDbModel>(request.UserId.ToString());
        if (products.Count > 0)
        {
            await db.GetTableClient(CartProductDbModel.GetTableName())
                .SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)), cancellationToken);
        }
    }
}


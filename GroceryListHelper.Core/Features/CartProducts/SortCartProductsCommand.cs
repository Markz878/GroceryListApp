namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record SortCartProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required ListSortDirection SortDirection { get; init; }

}

internal sealed class SortCartProductsCommandHandler(TableServiceClient db) : IRequestHandler<SortCartProductsCommand>
{
    public async Task Handle(SortCartProductsCommand request, CancellationToken cancellationToken = default)
    {
        TableClient client = db.GetTableClient(CartProductDbModel.GetTableName());
        List<CartProductDbModel> products = await client.GetTableEntitiesByPrimaryKey<CartProductDbModel>(request.UserId.ToString());
        if (products.Count > 1)
        {
            ProductSortMethods.SortProducts(products, request.SortDirection);
            await client.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)), cancellationToken);
        }
    }
}


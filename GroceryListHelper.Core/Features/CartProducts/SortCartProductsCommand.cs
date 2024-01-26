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
            SortProducts(products, request.SortDirection);
            await client.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)), cancellationToken);
        }
    }

    private static void SortProducts(List<CartProductDbModel> cartProducts, ListSortDirection sortDirection)
    {
        int order = 1000;
        if (sortDirection is ListSortDirection.Ascending)
        {
            foreach (CartProduct item in cartProducts.OrderBy(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
        else
        {
            foreach (CartProduct item in cartProducts.OrderByDescending(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
    }
}


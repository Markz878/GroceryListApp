namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record GetUserCartProductsQuery : IRequest<List<CartProductCollectable>>
{
    public required Guid UserId { get; init; }
}

internal sealed class GetUserCartProductsQueryHandler(TableServiceClient db) : IRequestHandler<GetUserCartProductsQuery, List<CartProductCollectable>>
{
    public async Task<List<CartProductCollectable>> Handle(GetUserCartProductsQuery request, CancellationToken cancellationToken = default)
    {
        List<CartProductDbModel> products = await db.GetTableClient(CartProductDbModel.GetTableName())
            .GetTableEntitiesByPrimaryKey<CartProductDbModel>(request.UserId.ToString());
        return products.Select(x => new CartProductCollectable()
        {
            Amount = x.Amount,
            IsCollected = x.IsCollected,
            Name = x.Name,
            Order = x.Order,
            UnitPrice = x.UnitPrice
        }).ToList();
    }
}


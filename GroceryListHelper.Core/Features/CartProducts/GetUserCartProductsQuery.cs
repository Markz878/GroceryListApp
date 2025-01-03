namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record GetUserCartProductsQuery : IRequest<List<CartProduct>>
{
    public required Guid UserId { get; init; }
}

internal sealed class GetUserCartProductsQueryHandler(CosmosClient db) : IRequestHandler<GetUserCartProductsQuery, List<CartProduct>>
{
    public async Task<List<CartProduct>> Handle(GetUserCartProductsQuery request, CancellationToken cancellationToken = default)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartProductsContainer);
        string sql = "SELECT * FROM c WHERE c.userId=@userId";
        QueryDefinition query = new QueryDefinition(sql)
            .WithParameter("@userId", request.UserId);
        List<CartProduct> products = await container.Query<CartProductEntity, CartProduct>(x => new CartProduct()
        {
            Amount = x.Amount,
            IsCollected = x.IsCollected,
            Name = x.Name,
            Order = x.Order,
            UnitPrice = x.UnitPrice
        }, query);
        return products;
    }
}


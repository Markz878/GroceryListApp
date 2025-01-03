namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record GetUserStoreProductsQuery : IRequest<List<StoreProduct>>
{
    public required Guid UserId { get; init; }
}

internal sealed class GetUserStoreProductsQueryHandler(CosmosClient db) : IRequestHandler<GetUserStoreProductsQuery, List<StoreProduct>>
{
    public async Task<List<StoreProduct>> Handle(GetUserStoreProductsQuery request, CancellationToken cancellationToken = default)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.StoreProductsContainer);
        string sql = "SELECT * FROM c WHERE c.userId=@userId";
        QueryDefinition query = new QueryDefinition(sql)
            .WithParameter("@userId", request.UserId);

        List<StoreProduct> result = await container.Query<StoreProductEntity, StoreProduct>(x => new StoreProduct() { Name = x.Name, UnitPrice = x.UnitPrice }, query);
        return result;
    }
}


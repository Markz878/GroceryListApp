namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record GetUserStoreProductsQuery : IRequest<List<StoreProduct>>
{
    public required Guid UserId { get; init; }
}

internal sealed class GetUserStoreProductsQueryHandler(TableServiceClient db) : IRequestHandler<GetUserStoreProductsQuery, List<StoreProduct>>
{
    public async Task<List<StoreProduct>> Handle(GetUserStoreProductsQuery request, CancellationToken cancellationToken = default)
    {
        List<StoreProductDbModel> result = await db.GetTableClient(StoreProductDbModel.GetTableName())
            .GetTableEntitiesByPrimaryKey<StoreProductDbModel>(request.UserId.ToString());
        return result.Select(x => new StoreProduct() { Name = x.Name, UnitPrice = x.UnitPrice }).ToList();
    }
}


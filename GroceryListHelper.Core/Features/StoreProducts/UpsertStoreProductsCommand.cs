namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record UpsertStoreProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required List<StoreProduct> StoreProducts { get; init; }
}

internal sealed class UpsertStoreProductsCommandHandler(CosmosClient db) : IRequestHandler<UpsertStoreProductsCommand>
{
    public async Task Handle(UpsertStoreProductsCommand request, CancellationToken cancellationToken = default)
    {
        if(request.StoreProducts.Count == 0)
        {
            return;
        }
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.StoreProductsContainer);
        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(request.UserId.ToString()));
        foreach (StoreProduct storeProduct in request.StoreProducts)
        {
            StoreProductEntity dbProduct = new()
            {
                Name = storeProduct.Name,
                UnitPrice = storeProduct.UnitPrice,
                UserId = request.UserId
            };
            batch.UpsertItem(dbProduct);
        }
        await batch.ExecuteAsync();
    }
}

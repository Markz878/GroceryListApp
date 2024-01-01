namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record AddStoreProductCommand : IRequest<ConflictError?>
{
    public required Guid UserId { get; init; }
    public required StoreProduct Product { get; init; }
}

internal sealed class AddStoreProductCommandHandler(TableServiceClient db) : IRequestHandler<AddStoreProductCommand, ConflictError?>
{
    public async Task<ConflictError?> Handle(AddStoreProductCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            StoreProductDbModel storeProduct = new()
            {
                Name = request.Product.Name,
                UnitPrice = request.Product.UnitPrice,
                OwnerId = request.UserId
            };
            await db.GetTableClient(StoreProductDbModel.GetTableName())
                .AddEntityAsync(storeProduct, cancellationToken);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 409)
        {
            return new ConflictError();
        }
    }
}


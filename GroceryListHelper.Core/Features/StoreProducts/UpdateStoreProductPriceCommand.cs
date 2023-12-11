namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record UpdateStoreProductPriceCommand : IRequest<NotFoundError?>
{
    public required string ProductName { get; init; }
    public required Guid UserId { get; init; }
    public required double Price { get; init; }
}

internal sealed class UpdateStoreProductPriceCommandHandler(TableServiceClient db) : IRequestHandler<UpdateStoreProductPriceCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(UpdateStoreProductPriceCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            await db.GetTableClient(StoreProductDbModel.GetTableName())
                .UpdateEntityAsync(new StoreProductDbModel()
                {
                    Name = request.ProductName,
                    OwnerId = request.UserId,
                    UnitPrice = request.Price
                }, ETag.All, cancellationToken: cancellationToken);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 404)
        {
            return new NotFoundError();
        }
    }
}

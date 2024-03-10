namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record UpdateProductCommand : IRequest<NotFoundError?>
{
    public required Guid UserId { get; init; }
    public required CartProduct CartProduct { get; init; }
}


internal sealed class UpdateProductCommandHandler(TableServiceClient db) : IRequestHandler<UpdateProductCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        CartProductDbModel dbProduct = new()
        {
            Amount = request.CartProduct.Amount,
            IsCollected = request.CartProduct.IsCollected,
            Name = request.CartProduct.Name,
            Order = request.CartProduct.Order,
            OwnerId = request.UserId,
            UnitPrice = request.CartProduct.UnitPrice
        };
        try
        {
            Response response = await db.GetTableClient(CartProductDbModel.GetTableName())
                .UpdateEntityAsync(dbProduct, ETag.All, TableUpdateMode.Merge, cancellationToken);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 404)
        {
            return new NotFoundError();
        }
    }
}


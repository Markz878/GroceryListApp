namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record AddCartProductCommand : IRequest<ConflictError?>
{
    public required Guid UserId { get; init; }
    public required CartProduct CartProduct { get; init; }
}

internal sealed class AddCartProductCommandHandler(TableServiceClient db) : IRequestHandler<AddCartProductCommand, ConflictError?>
{
    public async Task<ConflictError?> Handle(AddCartProductCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            CartProductDbModel cartDbProduct = new()
            {
                Name = request.CartProduct.Name,
                Order = request.CartProduct.Order,
                OwnerId = request.UserId,
                Amount = request.CartProduct.Amount,
                UnitPrice = request.CartProduct.UnitPrice
            };
            await db.GetTableClient(CartProductDbModel.GetTableName())
                .AddEntityAsync(cartDbProduct, cancellationToken);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 409)
        {
            return new ConflictError();
        }
    }
}


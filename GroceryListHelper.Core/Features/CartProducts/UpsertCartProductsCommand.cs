namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record UpsertCartProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required List<CartProduct> CartProducts { get; init; }
}

internal sealed class UpsertCartProductsCommandHandler(CosmosClient db) : IRequestHandler<UpsertCartProductsCommand>
{
    public async Task Handle(UpsertCartProductsCommand request, CancellationToken cancellationToken = default)
    {
        if(request.CartProducts.Count == 0)
        {
            return;
        }
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartProductsContainer);
        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(request.UserId.ToString()));
        foreach (CartProduct cartProduct in request.CartProducts)
        {
            CartProductEntity dbProduct = new()
            {
                Amount = cartProduct.Amount,
                IsCollected = cartProduct.IsCollected,
                Name = cartProduct.Name,
                Order = cartProduct.Order,
                UserId = request.UserId,
                UnitPrice = cartProduct.UnitPrice
            };
            batch.UpsertItem(dbProduct);
        }
        await batch.ExecuteAsync();
    }
}


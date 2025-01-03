namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record ClearUserCartProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
}

internal sealed class ClearCartProductsCommandHandler(CosmosClient db) : IRequestHandler<ClearUserCartProductsCommand>
{
    public async Task Handle(ClearUserCartProductsCommand request, CancellationToken cancellationToken = default)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartProductsContainer);
        await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(request.UserId.ToString()));
    }
}


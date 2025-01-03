namespace GroceryListHelper.Core.Features.StoreProducts;

public sealed record DeleteAllUserStoreProductsCommand : IRequest
{
    public required Guid UserId { get; init; }
}

internal sealed class DeleteAllUserStoreProductsCommandHandler(CosmosClient db) : IRequestHandler<DeleteAllUserStoreProductsCommand>
{
    public async Task Handle(DeleteAllUserStoreProductsCommand request, CancellationToken cancellationToken = default)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.StoreProductsContainer);
        await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(request.UserId.ToString()));
    }
}


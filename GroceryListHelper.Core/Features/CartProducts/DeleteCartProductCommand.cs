namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record DeleteCartProductCommand : IRequest<NotFoundError?>
{
    public required Guid UserId { get; init; }
    public required string ProductName { get; init; }
}

internal sealed class DeleteCartProductCommandHandler(CosmosClient db) : IRequestHandler<DeleteCartProductCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(DeleteCartProductCommand request, CancellationToken cancellationToken = default)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartProductsContainer);
        ResponseMessage response = await container.DeleteItemStreamAsync(request.ProductName, new PartitionKey(request.UserId.ToString()));
        return response.StatusCode == System.Net.HttpStatusCode.NotFound ? new NotFoundError() : null;
    }
}


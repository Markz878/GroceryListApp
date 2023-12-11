namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record DeleteCartProductCommand : IRequest<NotFoundError?>
{
    public required Guid UserId { get; init; }
    public required string ProductName { get; init; }
}

internal sealed class DeleteCartProductCommandHandler(TableServiceClient db) : IRequestHandler<DeleteCartProductCommand, NotFoundError?>
{
    public async Task<NotFoundError?> Handle(DeleteCartProductCommand request, CancellationToken cancellationToken = default)
    {
        Response response = await db.GetTableClient(CartProductDbModel.GetTableName())
            .DeleteEntityAsync(request.UserId.ToString(), request.ProductName);
        return response.Status == 404 ? new NotFoundError() : null;
    }
}


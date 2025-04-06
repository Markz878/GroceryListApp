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
        string sql = "SELECT * FROM c WHERE c.userId=@userId";
        QueryDefinition query = new QueryDefinition(sql)
            .WithParameter("@userId", request.UserId);
        List<CartProductEntity> products = await container.Query<CartProductEntity>(query);
        PartitionKey partitionKey = new(request.UserId.ToString());
        await container.BatchDelete(products.Select(x => x.Name), partitionKey);
    }
}


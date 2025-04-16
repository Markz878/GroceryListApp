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

        ContainerResponse containerResponse = await container.ReadContainerAsync();
        ContainerProperties containerProperties = containerResponse.Resource;
        containerProperties.DefaultTimeToLive = int.MaxValue;
        await container.ReplaceContainerAsync(containerProperties);

        string sql = "SELECT * FROM c WHERE c.userId=@userId";
        QueryDefinition query = new QueryDefinition(sql)
            .WithParameter("@userId", request.UserId);
        List<StoreProductEntity> products = await container.Query<StoreProductEntity>(query);
        PartitionKey partitionKey = new(request.UserId.ToString());
        await container.BatchDelete(products.Select(x => x.Name), partitionKey);
    }
}


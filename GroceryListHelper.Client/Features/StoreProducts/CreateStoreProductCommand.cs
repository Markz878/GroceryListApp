namespace GroceryListHelper.Client.Features.StoreProducts;

internal sealed record CreateStoreProductCommand : IRequest
{
    public required StoreProduct Product { get; init; }
}

internal sealed class CreateStoreProductCommandHandler(StoreProductsServiceProvider storeProductsServiceProvider) : IRequestHandler<CreateStoreProductCommand>
{
    public async Task Handle(CreateStoreProductCommand command, CancellationToken cancellationToken = default)
    {
        IStoreProductsService service = await storeProductsServiceProvider.ResolveStoreProductsService();
        await service.CreateStoreProduct(command.Product);
    }
}
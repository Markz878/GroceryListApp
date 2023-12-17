namespace GroceryListHelper.Client.Features.StoreProducts;

internal sealed record UpdateStoreProductCommand : IRequest
{
    public required StoreProduct Product { get; init; }
}

internal sealed class UpdateStoreProductCommandHandler(StoreProductsServiceProvider storeProductsServiceProvider) : IRequestHandler<UpdateStoreProductCommand>
{
    public async Task Handle(UpdateStoreProductCommand request, CancellationToken cancellationToken = default)
    {
        IStoreProductsService service = await storeProductsServiceProvider.ResolveStoreProductsService();
        await service.UpdateStoreProduct(request.Product);
    }
}

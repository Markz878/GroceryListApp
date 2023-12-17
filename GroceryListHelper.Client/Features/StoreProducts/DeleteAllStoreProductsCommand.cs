namespace GroceryListHelper.Client.Features.StoreProducts;

internal sealed record DeleteAllStoreProductsCommand : IRequest
{
}

internal sealed class DeleteAllStoreProductsCommandHandler(StoreProductsServiceProvider storeProductsServiceProvider) : IRequestHandler<DeleteAllStoreProductsCommand>
{
    public async Task Handle(DeleteAllStoreProductsCommand request, CancellationToken cancellationToken = default)
    {
        IStoreProductsService service = await storeProductsServiceProvider.ResolveStoreProductsService();
        await service.DeleteStoreProducts();
    }
}

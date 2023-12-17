namespace GroceryListHelper.Client.Features.StoreProducts;

internal sealed record GetStoreProductsQuery : IRequest<List<StoreProduct>>
{
}

internal sealed class GetStoreProductsQueryHandler(StoreProductsServiceProvider storeProductsServiceProvider) : IRequestHandler<GetStoreProductsQuery, List<StoreProduct>>
{
    public async Task<List<StoreProduct>> Handle(GetStoreProductsQuery request, CancellationToken cancellationToken = default)
    {
        IStoreProductsService service = await storeProductsServiceProvider.ResolveStoreProductsService();
        return await service.GetStoreProducts();
    }
}

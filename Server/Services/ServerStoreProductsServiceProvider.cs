namespace GroceryListHelper.Server.Services;

public sealed class ServerStoreProductsServiceProvider : IStoreProductsService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IStoreProductRepository storeProductRepository;

    public ServerStoreProductsServiceProvider(IHttpContextAccessor httpContextAccessor, IStoreProductRepository storeProductRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.storeProductRepository = storeProductRepository;
    }
    public Task ClearStoreProducts()
    {
        throw new NotImplementedException();
    }

    public async Task<List<StoreProduct>> GetStoreProducts()
    {
        Guid? userName = httpContextAccessor.HttpContext?.User.GetUserId();
        if (userName == null)
        {
            return new List<StoreProduct>();
        }
        List<StoreProduct> storeProducts = await storeProductRepository.GetStoreProductsForUser(userName.Value);
        return storeProducts;
    }

    public Task SaveStoreProduct(StoreProduct product)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStoreProduct(StoreProduct storeProduct)
    {
        throw new NotImplementedException();
    }
}

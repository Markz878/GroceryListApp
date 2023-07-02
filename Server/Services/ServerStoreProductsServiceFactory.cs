namespace GroceryListHelper.Server.Services;

public class ServerStoreProductsServiceFactory : IStoreProductsServiceFactory
{
    private readonly IEnumerable<IStoreProductsService> storeProductsServices;

    public ServerStoreProductsServiceFactory(IEnumerable<IStoreProductsService> storeProductsServices)
    {
        this.storeProductsServices = storeProductsServices;
    }

    public Task<IStoreProductsService> GetStoreProductsService()
    {
        IStoreProductsService service = storeProductsServices.OfType<ServerStoreProductsServiceProvider>().First();
        return Task.FromResult(service);
    }
}
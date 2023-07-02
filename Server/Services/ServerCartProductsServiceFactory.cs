namespace GroceryListHelper.Server.Services;

public class ServerCartProductsServiceFactory : ICartProductsServiceFactory
{
    private readonly IEnumerable<ICartProductsService> cartProductsServices;

    public ServerCartProductsServiceFactory(IEnumerable<ICartProductsService> cartProductsServices)
    {
        this.cartProductsServices = cartProductsServices;
    }
    public Task<ICartProductsService> GetCartProductsService()
    {
        ICartProductsService service = cartProductsServices.OfType<ServerCartProductsServiceProvider>().First();
        return Task.FromResult(service);
    }
}

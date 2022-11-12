using Mapster;

namespace GroceryListHelper.Server.Services;

public class HostCartProductsServiceProvider : ICartProductsService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICartProductRepository cartProductsRepository;

    public HostCartProductsServiceProvider(IHttpContextAccessor httpContextAccessor, ICartProductRepository cartProductsRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.cartProductsRepository = cartProductsRepository;
    }

    public Task DeleteAllCartProducts()
    {
        throw new NotImplementedException();
    }

    public Task DeleteCartProduct(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        Guid? userName = httpContextAccessor.HttpContext?.User.GetUserId();
        if (userName == null)
        {
            return new List<CartProductUIModel>();
        }
        List<CartProductCollectable> cartProducts = await cartProductsRepository.GetCartProductsForUser(userName.Value);
        return cartProducts.Select(x => x.Adapt<CartProductUIModel>()).ToList();
    }

    public Task<Guid> SaveCartProduct(CartProduct product)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        throw new NotImplementedException();
    }
}

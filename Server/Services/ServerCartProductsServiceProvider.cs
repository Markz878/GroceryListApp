using System.ComponentModel;

namespace GroceryListHelper.Server.Services;

public sealed class ServerCartProductsServiceProvider : ICartProductsService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICartProductRepository cartProductsRepository;

    public ServerCartProductsServiceProvider(IHttpContextAccessor httpContextAccessor, ICartProductRepository cartProductsRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.cartProductsRepository = cartProductsRepository;
    }

    public Task DeleteAllCartProducts()
    {
        throw new NotImplementedException();
    }

    public Task DeleteCartProduct(string name)
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
        List<CartProductCollectable> cartProducts = await cartProductsRepository.GetCartProducts(userName.Value);
        return cartProducts.Select(x => new CartProductUIModel()
        {
            Amount = x.Amount,
            IsCollected = x.IsCollected,
            Name = x.Name,
            Order = x.Order,
            UnitPrice = x.UnitPrice
        }).ToList();
    }

    public Task SaveCartProduct(CartProduct product)
    {
        throw new NotImplementedException();
    }

    public Task SortCartProducts(ListSortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        throw new NotImplementedException();
    }
}

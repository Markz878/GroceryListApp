namespace GroceryListHelper.Client.Services;

public sealed class CartProductsSignalRService : ICartProductsService
{
    private readonly ICartHubClient cartHubClient;

    public CartProductsSignalRService(ICartHubClient cartHubClient)
    {
        this.cartHubClient = cartHubClient;
    }

    public Task DeleteAllCartProducts()
    {
        throw new NotImplementedException();
    }

    public Task DeleteCartProduct(string name)
    {
        return cartHubClient.CartItemDeleted(name);
    }

    public Task<List<CartProductUIModel>> GetCartProducts()
    {
        throw new NotImplementedException();
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        await cartHubClient.CartItemAdded(product);
    }

    public Task SortCartProducts(ListSortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCartProduct(CartProductCollectable product)
    {
        return cartHubClient.CartItemModified(product);
    }
}

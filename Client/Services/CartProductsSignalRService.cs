using System.ComponentModel;

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

    public Task DeleteCartProduct(Guid id)
    {
        return cartHubClient.CartItemDeleted(id);
    }

    public Task<List<CartProductUIModel>> GetCartProducts()
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> SaveCartProduct(CartProduct product)
    {
        HubResponse response = await cartHubClient.CartItemAdded(product);
        return string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.SuccessMessage)
            ? Guid.Parse(response.SuccessMessage)
            : throw new Exception(response.ErrorMessage);
    }

    public Task SortCartProducts(ListSortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCartProduct(CartProductUIModel product)
    {
        return cartHubClient.CartItemModified(product);
    }
}

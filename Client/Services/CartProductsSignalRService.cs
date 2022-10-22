using GroceryListHelper.Shared.Models.BaseModels;

namespace GroceryListHelper.Client.Services;

public class CartProductsSignalRService : ICartProductsService
{
    private readonly HubConnection cartHub;

    public CartProductsSignalRService(HubConnection cartHub)
    {
        this.cartHub = cartHub;
    }

    public Task DeleteAllCartProducts()
    {
        throw new NotImplementedException();
    }

    public Task DeleteCartProduct(Guid id)
    {
        return cartHub.SendAsync(nameof(ICartHubActions.CartItemDeleted), id);
    }

    public Task<List<CartProductUIModel>> GetCartProducts()
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> SaveCartProduct(CartProduct product)
    {
        HubResponse response = await cartHub.InvokeAsync<HubResponse>(nameof(ICartHubActions.CartItemAdded), product);
        return string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.SuccessMessage)
            ? Guid.Parse(response.SuccessMessage)
            : throw new Exception(response.ErrorMessage);
    }

    public Task UpdateCartProduct(CartProductUIModel product)
    {
        return cartHub.SendAsync(nameof(ICartHubActions.CartItemModified), product);
    }
}

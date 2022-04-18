using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.BaseModels;
using GroceryListHelper.Shared.Models.CartProduct;
using Microsoft.AspNetCore.SignalR.Client;

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
        if (string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.SuccessMessage))
        {
            return Guid.Parse(response.SuccessMessage);
        }
        throw new Exception(response.ErrorMessage);
    }

    public Task UpdateCartProduct(CartProductUIModel product)
    {
        return cartHub.SendAsync(nameof(ICartHubActions.CartItemModified), product);
    }
}

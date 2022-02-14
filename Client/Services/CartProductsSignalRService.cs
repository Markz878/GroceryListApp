using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace GroceryListHelper.Client.Services;

public class CartProductsSignalRService : ICartProductsService
{
    private readonly IndexViewModel viewModel;

    public CartProductsSignalRService(IndexViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task DeleteAllCartProducts()
    {
        viewModel.CartProducts.Clear();
        return Task.CompletedTask;
    }

    public Task DeleteCartProduct(string id)
    {
        return viewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemDeleted), id);
    }

    public Task<List<CartProductUIModel>> GetCartProducts()
    {
        return Task.FromResult(viewModel.CartProducts.ToList());
    }

    public async Task SaveCartProduct(CartProductUIModel product)
    {
        product.Id = await viewModel.CartHub.InvokeAsync<string>(nameof(ICartHubActions.CartItemAdded), product);
    }

    public Task UpdateCartProduct(CartProductUIModel product)
    {
        return viewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemModified), product);
    }
}

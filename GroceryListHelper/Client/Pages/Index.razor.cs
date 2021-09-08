using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    public class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; }
        [Inject] public CartHubBuilder CartHubBuilder { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TypeAdapterConfig<CartProductCollectable, CartProductUIModel>.NewConfig().ConstructUsing(x => new CartProductUIModel(ViewModel));
            ViewModel.CartProducts.Clear();
            foreach (CartProductCollectable item in await CartProductsService.GetCartProducts())
            {
                ViewModel.CartProducts.Add(item.Adapt<CartProductUIModel>());
            }
            ViewModel.StoreProducts.Clear();
            foreach (StoreProductUIModel item in await StoreProductsService.GetStoreProducts())
            {
                ViewModel.StoreProducts.Add(item);
            }
            CartHubBuilder.BuildCartHubConnection();
        }

        public async ValueTask DisposeAsync()
        {
            await ViewModel?.CartHub?.StopAsync();
        }
    }
}

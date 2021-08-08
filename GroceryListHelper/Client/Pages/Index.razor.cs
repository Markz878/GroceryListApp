using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewModel.CartProducts.Clear();
            foreach (CartProductUIModel item in await CartProductsService.GetCartProducts())
            {
                ViewModel.CartProducts.Add(item);
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

using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Components
{
    public class CartSummaryRowComponentBase : BasePage<IndexViewModel>
    {
        [Inject] public CartProductsService CartProductsService { get; set; }
        [Inject] public StoreProductsService StoreProductsService { get; set; } 
        public bool AllCollected => ViewModel.CartProducts.All(x => x.IsCollected);
        public double Total => ViewModel.CartProducts.Sum(x => x.Total);

        public Task ClearCartProducts()
        {
            ViewModel.CartProducts.Clear();
            return CartProductsService.ClearCartProducts();
        }

        public Task ClearStoreProducts()
        {
            ViewModel.StoreProducts.Clear();
            return StoreProductsService.ClearStoreProducts();
        }
    }
}
using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GroceryListHelper.Client.Pages;

public class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
{
    //[Inject] public CartProductsApiService CartProductsService { get; set; }
    //[Inject] public StoreProductsService StoreProductsService { get; set; }
    [Inject] public CartHubBuilder CartHubBuilder { get; set; }

    protected override void OnInitialized()
    {
        CartHubBuilder.BuildCartHubConnection();
    }

    public async ValueTask DisposeAsync()
    {
        await ViewModel?.CartHub?.StopAsync();
    }
}

using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GroceryListHelper.Client.Pages;

public class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
{
    [Inject] public CartHubBuilder CartHubBuilder { get; set; }

    protected override void OnInitialized()
    {
        CartHubBuilder.BuildCartHubConnection();
        base.OnInitialized();
    }

    public async ValueTask DisposeAsync()
    {
        await ViewModel?.CartHub?.StopAsync();
    }
}

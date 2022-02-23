using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GroceryListHelper.Client.Pages;

public class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
{
    [Inject] public CartHubBuilder CartHubBuilder { get; set; }
    [Inject] public IJSRuntime Js { get; set; }
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    protected override void OnInitialized()
    {
        CartHubBuilder.BuildCartHubConnection();
        base.OnInitialized();
    }

    public async ValueTask DisposeAsync()
    {
        await ViewModel?.CartHub?.StopAsync();
        GC.SuppressFinalize(this);
    }
}

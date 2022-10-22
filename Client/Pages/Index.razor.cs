using GroceryListHelper.Shared.Models.Authentication;

namespace GroceryListHelper.Client.Pages;

public class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
{
    [Inject] public ICartHubBuilder CartHubBuilder { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public PersistentComponentState ApplicationState { get; set; } = default!;
    private PersistingComponentStateSubscription stateSubscription;
    protected UserInfo? userInfo;

    protected override async void OnInitialized()
    {
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        if (!ApplicationState.TryTakeFromJson(nameof(userInfo), out userInfo))
        {
            userInfo = await AuthenticationStateProvider.GetUserInfo();
        }
        CartHubBuilder.BuildCartHubConnection();
        base.OnInitialized();
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(userInfo), userInfo);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (ViewModel.CartHub is not null)
        {
            await ViewModel.CartHub.StopAsync();
            CartHubBuilder.Dispose();
        }
        stateSubscription.Dispose();
        GC.SuppressFinalize(this);
    }
}

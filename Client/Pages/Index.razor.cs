using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Client.Pages;

public abstract class IndexBase : BasePage<IndexViewModel>, IAsyncDisposable
{
    [Inject] public ICartHubClient CartHubClient { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public PersistentComponentState ApplicationState { get; set; } = default!;
    [Inject] public required RenderLocation RenderLocation { get; set; }
    private PersistingComponentStateSubscription stateSubscription;
    protected UserInfo? userInfo;

    protected override async Task OnInitializedAsync()
    {
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        if (!ApplicationState.TryTakeFromJson(nameof(userInfo), out userInfo))
        {
            userInfo = await AuthenticationStateProvider.GetUserInfo();
        }
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(userInfo), userInfo);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        stateSubscription.Dispose();
        await CartHubClient.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}

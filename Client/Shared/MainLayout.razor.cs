namespace GroceryListHelper.Client.Shared;

public sealed partial class MainLayout : IDisposable
{
    [Inject] public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }

    private PersistingComponentStateSubscription stateSubscription;
    private UserInfo? loggedInUserInfo;

    protected override async Task OnInitializedAsync()
    {
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        if (!ApplicationState.TryTakeFromJson(nameof(loggedInUserInfo), out loggedInUserInfo))
        {
            loggedInUserInfo = await AuthenticationStateProvider.GetUserInfo();
        }
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(loggedInUserInfo), loggedInUserInfo);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        stateSubscription.Dispose();
    }
}

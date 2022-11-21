namespace GroceryListHelper.Client.Shared;

public partial class LoginDisplay : IDisposable
{
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public PersistentComponentState ApplicationState { get; set; } = default!;
    private PersistingComponentStateSubscription stateSubscription;
    protected UserInfo? loggedInUserInfo;

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

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

    private string GetUserName()
    {
        string name = loggedInUserInfo?.Claims?.FirstOrDefault(x => x.Type == "name")?.Value
                    ?? loggedInUserInfo?.Claims?.FirstOrDefault(x => x.Type == "preferred_username")?.Value
                    ?? "Shopper";
        return name;
    }
    public void Dispose()
    {
        stateSubscription.Dispose();
    }
}

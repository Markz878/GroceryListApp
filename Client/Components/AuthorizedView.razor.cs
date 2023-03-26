namespace GroceryListHelper.Client.Components;

public partial class AuthorizedView : IDisposable
{
    [Inject] public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }
    [Parameter][EditorRequired] public required RenderFragment ChildContent { get; set; }
    [Parameter] public Func<Task<bool>>? AuthorizedCondition { get; set; }

    private UserInfo? userInfo;
    private PersistingComponentStateSubscription stateSubscription;

    protected override async Task OnInitializedAsync()
    {
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        if (!ApplicationState.TryTakeFromJson(nameof(AuthorizedView) + nameof(userInfo), out userInfo))
        {
            userInfo = await AuthenticationStateProvider.GetUserInfo();
            if (AuthorizedCondition is not null && await AuthorizedCondition.Invoke() is false)
            {
                userInfo.IsAuthenticated = false;
            }
        }
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(AuthorizedView) + nameof(userInfo), userInfo);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        stateSubscription.Dispose();
        GC.SuppressFinalize(this);
    }
}
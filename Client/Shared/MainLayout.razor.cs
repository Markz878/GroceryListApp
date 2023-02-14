namespace GroceryListHelper.Client.Shared;

public partial class MainLayout : IDisposable
{
    [Inject] public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    private PersistingComponentStateSubscription stateSubscription;
    protected UserInfo? loggedInUserInfo;
    protected bool userMenuOpen;

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

    private void ToggleUserMenuOpen()
    {
        userMenuOpen = !userMenuOpen;
    }

    protected void NavigateToGroups()
    {
        Navigation.NavigateTo("/managegroups");
    }

    protected void NavigateToIndex()
    {
        Navigation.NavigateTo("");
    }

    public static string? GetUserInitials(UserInfo? userAuthInfo)
    {
        if (userAuthInfo is null || !userAuthInfo.IsAuthenticated)
        {
            return null;
        }
        string? name = userAuthInfo.Claims?.FirstOrDefault(x => x.Type == "name")?.Value;
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }
        string[] names = name.Split(" ");
        return new string(names.Select(x => x[0]).ToArray());
    }

    public void Dispose()
    {
        stateSubscription.Dispose();
    }
}

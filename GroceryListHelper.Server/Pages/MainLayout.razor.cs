namespace GroceryListHelper.Server.Pages;

public sealed partial class MainLayout
{
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; set; }

    private UserInfo? _loggedInUserInfo;

    protected override async Task OnInitializedAsync()
    {
        _loggedInUserInfo = await AuthenticationStateTask.GetUserInfo();
    }
}

namespace GroceryListHelper.Server.Pages.Components;

[Authorize]
public sealed partial class ProfileButton
{
    [CascadingParameter] public required Task<AuthenticationState> AuthenticationStateTask { get; init; }
    [Inject] public required NavigationManager Navigation { get; init; }

    private string? _email;
    private string? _userName;

    protected override async Task OnInitializedAsync()
    {
        UserInfo userInfo = await AuthenticationStateTask.GetUserInfo();
        _email = userInfo.GetUserEmail();
        _userName = userInfo.GetUserName();
    }

    internal static string GetUserInitials(string? userName, string? email)
    {
        if (string.IsNullOrWhiteSpace(userName) is false)
        {
            string[] names = userName.ToUpper().Split(" ", 2, StringSplitOptions.TrimEntries);
            if (names.Length == 2)
            {
                return new string(names.Select(x => x[0]).ToArray());
            }
            return userName[0..2].ToUpperInvariant();
        }
        if (string.IsNullOrWhiteSpace(email) is false)
        {
            return email[0..2].ToUpperInvariant();
        }
        return "U";
    }
}
namespace GroceryListHelper.Client.Components;

public partial class UserInfoButton
{
    [Parameter][EditorRequired] public required UserInfo UserInfo { get; set; }
    
    protected bool userMenuOpen;

    private void ToggleUserMenuOpen()
    {
        userMenuOpen = !userMenuOpen;
    }

    private static string? GetUserInitials(UserInfo? userAuthInfo)
    {
        if (userAuthInfo is null || !userAuthInfo.IsAuthenticated)
        {
            return null;
        }
        string? name = userAuthInfo.Claims?.FirstOrDefault(x => x.Type == "name")?.Value;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = userAuthInfo.Claims?.FirstOrDefault(x => x.Type == "preferred_username")?.Value;
            if (name is null)
            {
                return null;
            }
        }
        string[] names = name.ToUpper().Split(" ", 2, StringSplitOptions.TrimEntries);
        return new string(names.Select(x => x[0]).ToArray());
    }
}

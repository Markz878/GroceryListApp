namespace GroceryListHelper.Client.Authentication;

public static class AuthenticationStateExtensions
{
    public static async Task<bool> IsUserAuthenticated(this AuthenticationStateProvider authenticationStateProvider)
    {
        AuthenticationState authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User?.Identity?.IsAuthenticated == true;
    }

    public static async Task<UserInfo> GetUserInfo(this AuthenticationStateProvider authenticationStateProvider)
    {
        AuthenticationState authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return new UserInfo()
        {
            IsAuthenticated = authenticationState.User.Identity?.IsAuthenticated == true,
            Claims = authenticationState.User.Claims.Select(x => new ClaimValue(x.Type, x.Value)).ToList(),
        };
    }
}

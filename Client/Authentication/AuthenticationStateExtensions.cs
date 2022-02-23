using Microsoft.AspNetCore.Components.Authorization;

namespace GroceryListHelper.Client.Authentication;

public static class AuthenticationStateExtensions
{
    public static async Task<bool> IsUserAuthenticated(this AuthenticationStateProvider authenticationStateProvider)
    {
        AuthenticationState authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User?.Identity?.IsAuthenticated == true;
    }
}

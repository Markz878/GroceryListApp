using System.Security.Claims;

namespace GroceryListHelper.Client.Authentication;

public sealed class ClientAuthenticationStateProvider(IHttpClientFactory httpClientFactory) : AuthenticationStateProvider
{
    private AuthenticationState? cachedAuthState;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (cachedAuthState is null)
        {
            UserInfo? userInfo = await httpClientFactory.CreateClient("Client").GetFromJsonAsync<UserInfo>("api/account/user");
            if (userInfo is not null && userInfo.IsAuthenticated)
            {
                Claim[] claims = [
                    new Claim(AuthenticationConstants.IdClaimName, userInfo.GetUserId().ToString() ?? ""),
                    new Claim(AuthenticationConstants.EmailClaimName, userInfo.GetUserEmail() ?? ""),
                    new Claim(AuthenticationConstants.NameClaimName, userInfo.GetUserName() ?? "")];
                cachedAuthState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
            }
            else
            {
                cachedAuthState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }


        return cachedAuthState;
    }
}
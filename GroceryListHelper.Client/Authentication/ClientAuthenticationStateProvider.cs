using System.Security.Claims;

namespace GroceryListHelper.Client.Authentication;

public sealed class ClientAuthenticationStateProvider(PersistentComponentState persistentState) : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> _unauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!persistentState.TryTakeFromJson(nameof(UserInfo), out UserInfo? userInfo) || userInfo is null)
        {
            return _unauthenticatedTask;
        }

        Claim[] claims = [
            new Claim(AuthenticationConstants.IdClaimName, userInfo.GetUserId().ToString() ?? ""),
            new Claim(AuthenticationConstants.EmailClaimName, userInfo.GetUserEmail() ?? ""),
            new Claim(AuthenticationConstants.NameClaimName, userInfo.GetUserName() ?? "")];

        return Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"))));
    }
}
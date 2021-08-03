using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("GroceryListHelper.Tests")]
namespace GroceryListHelper.Client.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAccessTokenProvider accessTokenProvider;

        public CustomAuthenticationStateProvider(IAccessTokenProvider accessTokenProvider)
        {
            this.accessTokenProvider = accessTokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await accessTokenProvider.RequestAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                ClaimsIdentity claimsIdentity = new(token.ParseTokenClaims(), "BearerToken");
                ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
                AuthenticationState authenticationState = new(claimsPrincipal);
                NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
                return authenticationState;
            }
            else
            {
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }
    }

    public static class AuthenticationStateProviderExtensions
    {
        public static async Task<bool> IsUserAuthenticated(this AuthenticationStateProvider authenticationStateProvider)
        {
            AuthenticationState authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            return authenticationState.User?.Identity?.IsAuthenticated == true;
        }
    }
}

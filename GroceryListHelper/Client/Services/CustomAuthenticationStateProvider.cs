using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
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
            Console.WriteLine("Geting auth state");
            AccessTokenResult tokenResult = await accessTokenProvider.RequestAccessToken();
            if (tokenResult.Status == AccessTokenResultStatus.Success && (tokenResult.TryGetToken(out AccessToken token)))
            {
                JwtSecurityTokenHandler tokenHandler = new();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token.Value);
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "BearerToken");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                AuthenticationState authenticationState = new AuthenticationState(claimsPrincipal);
                NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
                return authenticationState;
            }
            else
            {
                NotifyLogOut();
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        public void NotifyLogOut()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }
    }
}

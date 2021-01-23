using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
{
    public class CustomAccessTokenProvider : IAccessTokenProvider
    {
        private string accessToken;
        private readonly HttpClient client;
        private static AccessTokenResult RedirectResult => new(AccessTokenResultStatus.RequiresRedirect, null, "/login");

        public CustomAccessTokenProvider(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("AnonymousClient");
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            JwtSecurityTokenHandler tokenHandler = new();
            if(string.IsNullOrEmpty(accessToken))
            {
                if (await TryToRefreshToken())
                {
                    JwtSecurityToken jwtAccessToken = tokenHandler.ReadJwtToken(accessToken);
                    return new AccessTokenResult(AccessTokenResultStatus.Success,
                        new AccessToken() { Expires = jwtAccessToken.ValidTo, GrantedScopes = jwtAccessToken.Claims.Select(x => x.Type).ToArray(), Value = accessToken },
                        string.Empty);
                }
                return RedirectResult;
            }
            else
            {
                JwtSecurityToken jwtAccessToken = tokenHandler.ReadJwtToken(accessToken);
                if (jwtAccessToken.ValidTo.ToUniversalTime() < DateTime.UtcNow)
                {
                    if (await TryToRefreshToken())
                    {
                        jwtAccessToken = tokenHandler.ReadJwtToken(accessToken);
                        return new AccessTokenResult(AccessTokenResultStatus.Success,
                            new AccessToken() { Expires = jwtAccessToken.ValidTo, GrantedScopes = jwtAccessToken.Claims.Select(x => x.Type).ToArray(), Value = accessToken },
                            string.Empty);
                    }
                    return RedirectResult;
                }
                else
                {
                    return new AccessTokenResult(AccessTokenResultStatus.Success,
                        new AccessToken() { Expires = jwtAccessToken.ValidTo, GrantedScopes = jwtAccessToken.Claims.Select(x => x.Type).ToArray(), Value = accessToken },
                        string.Empty);
                }
            }

        }

        private async Task<bool> TryToRefreshToken()
        {
            var response = await client.GetAsync("api/authentication/refresh");
            if (response.IsSuccessStatusCode)
            {
                LoginResponseModel token = await response.Content.ReadFromJsonAsync<LoginResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                accessToken = token.AccessToken;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveAccessToken(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

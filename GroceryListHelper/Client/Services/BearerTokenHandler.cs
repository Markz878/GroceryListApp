using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
{
    public sealed class BearerTokenHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider tokenProvider;
        private readonly NavigationManager navigation;

        public BearerTokenHandler(IAccessTokenProvider tokenProvider, NavigationManager navigation)
        {
            this.tokenProvider = tokenProvider;
            this.navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.Status == AccessTokenResultStatus.RequiresRedirect)
            {
                throw new AccessTokenNotAvailableException(navigation, tokenResult, Array.Empty<string>());
            }
            tokenResult.TryGetToken(out AccessToken accessToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);
            return await base.SendAsync(request, token);
        }
    }
}

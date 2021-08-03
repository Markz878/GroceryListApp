using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Authentication
{
    public sealed class ProtectedClientAuthorizationHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider tokenProvider;
        private readonly NavigationManager navigation;

        public ProtectedClientAuthorizationHandler(IAccessTokenProvider tokenProvider, NavigationManager navigation)
        {
            this.tokenProvider = tokenProvider;
            this.navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string accessToken = await tokenProvider.RequestAccessToken();
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                accessToken = await tokenProvider.TryToRefreshToken();
                if (string.IsNullOrEmpty(accessToken))
                {
                    navigation.NavigateTo("/login", true);
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = await base.SendAsync(request, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    navigation.NavigateTo("/login", true);
                }
            }
            return response;
        }
    }
}

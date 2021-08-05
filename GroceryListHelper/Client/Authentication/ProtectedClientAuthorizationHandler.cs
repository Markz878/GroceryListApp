using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ModalViewModel modal;

        public ProtectedClientAuthorizationHandler(IAccessTokenProvider tokenProvider,
                                                   NavigationManager navigation,
                                                   AuthenticationStateProvider authenticationStateProvider,
                                                   ModalViewModel modal)
        {
            this.tokenProvider = tokenProvider;
            this.navigation = navigation;
            this.authenticationStateProvider = authenticationStateProvider;
            this.modal = modal;
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
                    await Logout();
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = await base.SendAsync(request, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await Logout();
                }
            }
            return response;
        }

        private async Task Logout()
        {
            await tokenProvider.RemoveToken();
            await authenticationStateProvider.GetAuthenticationStateAsync();
            modal.Message = "Your session has expired, please login.";
            await Task.Delay(1500);
            navigation.NavigateTo("/login", true);
        }
    }
}

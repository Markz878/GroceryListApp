using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Headers;

namespace GroceryListHelper.Client.Authentication;

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
        HttpResponseMessage response = new(HttpStatusCode.Unauthorized);
        if (string.IsNullOrEmpty(accessToken))
        {
            await HandleSessionExpired();
            return response;
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await HandleSessionExpired();
            return response;
        }
        return response;
    }

    private async Task HandleSessionExpired()
    {
        await tokenProvider.RemoveToken();
        await authenticationStateProvider.GetAuthenticationStateAsync();
        modal.Message = "Your session has expired, please login.";
        await Task.Delay(2000);
        navigation.NavigateTo("/login", true);
    }
}

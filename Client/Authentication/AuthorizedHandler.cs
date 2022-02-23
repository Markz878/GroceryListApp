using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;

namespace GroceryListHelper.Client.Authentication;

public class AuthorizedHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager navigation;
    private const string LogInPath = "api/Account/Login";

    public AuthorizedHandler(AuthenticationStateProvider authenticationStateProvider, NavigationManager navigation)
    {
        _authenticationStateProvider = authenticationStateProvider;
        this.navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;
        if (!authState.User.Identity.IsAuthenticated)
        {
            responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        else
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }
        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            string encodedReturnUrl = Uri.EscapeDataString(navigation.Uri);
            Uri logInUrl = navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
            navigation.NavigateTo(logInUrl.ToString(), true);
        }
        return responseMessage;
    }
}

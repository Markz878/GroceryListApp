using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net;

namespace GroceryListHelper.Client.Authentication;

public class AuthorizedHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime js;

    public AuthorizedHandler(AuthenticationStateProvider authenticationStateProvider, IJSRuntime js)
    {
        _authenticationStateProvider = authenticationStateProvider;
        this.js = js;
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
            string token = await js.InvokeAsync<string>("getAntiForgeryToken");
            request.Headers.Add("X-XSRF-TOKEN", token);
            responseMessage = await base.SendAsync(request, cancellationToken);
        }
        return responseMessage;
    }
}

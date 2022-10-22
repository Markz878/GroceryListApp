using System.Net;

namespace GroceryListHelper.Client.Authentication;

public class AuthorizedHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public AuthorizedHandler(AuthenticationStateProvider authenticationStateProvider)
    {
        this.authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage = !authState.User.Identity.IsAuthenticated
            ? new HttpResponseMessage(HttpStatusCode.Unauthorized)
            : await base.SendAsync(request, cancellationToken);
        return responseMessage;
    }
}

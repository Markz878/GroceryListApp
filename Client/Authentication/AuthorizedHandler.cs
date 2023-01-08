namespace GroceryListHelper.Client.Authentication;

public sealed class AuthorizedHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly IJSRuntime js;

    public AuthorizedHandler(AuthenticationStateProvider authenticationStateProvider, IJSRuntime js)
    {
        this.authenticationStateProvider = authenticationStateProvider;
        this.js = js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;
        if (!authState.User.Identity?.IsAuthenticated == true)
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

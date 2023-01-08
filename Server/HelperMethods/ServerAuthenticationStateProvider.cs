namespace GroceryListHelper.Server.HelperMethods;

public sealed class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ServerAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal()));
    }
}

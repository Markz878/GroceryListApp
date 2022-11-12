namespace GroceryListHelper.Server.HelperMethods;

public class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HostAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal()));
    }
}

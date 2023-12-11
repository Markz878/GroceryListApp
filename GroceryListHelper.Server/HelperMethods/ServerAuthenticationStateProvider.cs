using Microsoft.AspNetCore.Components.Web;

namespace GroceryListHelper.Server.HelperMethods;

public sealed class ServerAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;

    public ServerAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor, PersistentComponentState state)
    {
        _httpContextAccessor = httpContextAccessor;
        _state = state;
        _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_httpContextAccessor.HttpContext?.User ?? new()));
    }

    private Task OnPersistingAsync()
    {
        ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated == true)
        {
            string? userId = principal.FindFirst(AuthenticationConstants.IdClaimName)?.Value;
            string? email = principal.FindFirst(AuthenticationConstants.EmailClaimName)?.Value;

            if (userId != null && email != null)
            {
                _state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    IsAuthenticated = true,
                    Claims = principal.Claims.Select(x => new ClaimValue(x.Type, x.Value)).ToArray(),
                });
            }
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}
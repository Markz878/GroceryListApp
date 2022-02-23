using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GroceryListHelper.Client.Authentication;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
public class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private const string LogInPath = "api/Account/Login";
    private const string LogOutPath = "api/Account/Logout";

    private readonly NavigationManager _navigation;
    private readonly HttpClient _client;
    private readonly ILogger<HostAuthenticationStateProvider> _logger;

    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public HostAuthenticationStateProvider(NavigationManager navigation, IHttpClientFactory clientFactory, ILogger<HostAuthenticationStateProvider> logger)
    {
        _navigation = navigation;
        _client = clientFactory.CreateClient("AnonymousClient");
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUser(useCache: true));
    }

    public void SignIn(string customReturnUrl = null)
    {
        string returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
        string encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
        Uri logInUrl = _navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
        _navigation.NavigateTo(logInUrl.ToString(), true);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
    {
        DateTimeOffset now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
        {
            _logger.LogInformation("Taking user from cache");
            return _cachedUser;
        }

        _logger.LogInformation("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

        return _cachedUser;
    }

    private async Task<ClaimsPrincipal> FetchUser()
    {
        UserInfo user = null;

        try
        {
            user = await _client.GetFromJsonAsync<UserInfo>("api/User");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fetching user failed.");
        }

        if (user == null || !user.IsAuthenticated)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        ClaimsIdentity identity = new ClaimsIdentity(
            nameof(HostAuthenticationStateProvider),
            user.NameClaimType,
            user.RoleClaimType);

        if (user.Claims != null)
        {
            foreach (ClaimValue claim in user.Claims)
            {
                identity.AddClaim(new Claim(claim.Type, claim.Value));
            }
        }

        return new ClaimsPrincipal(identity);
    }
}

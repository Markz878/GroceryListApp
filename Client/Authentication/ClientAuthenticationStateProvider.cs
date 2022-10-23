namespace GroceryListHelper.Client.Authentication;

public class ClientAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);
    private readonly HttpClient _client;
    private readonly ILogger<ClientAuthenticationStateProvider> _logger;
    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public ClientAuthenticationStateProvider(IHttpClientFactory clientFactory, ILogger<ClientAuthenticationStateProvider> logger)
    {
        _client = clientFactory.CreateClient("AnonymousClient");
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUser(useCache: true));
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
        UserInfo? user = null;

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

        ClaimsIdentity identity = new("Cookies", "name", "role");

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

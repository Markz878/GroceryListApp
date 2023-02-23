namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static Guid UserId { get; } = Guid.NewGuid();
    public const string UserEmail = "test_user@email.com";
    public const string RandomEmail1 = "test1@email.com";
    public const string RandomEmail2 = "test2@email.com";
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims = new[]
        {
            new Claim("name", "Test user"),
            new Claim("preferred_username", UserEmail),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", UserId.ToString())
        };
        ClaimsIdentity identity = new(claims, "FakeAuth", "preferred_username", "role");
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, "FakeAuth");
        AuthenticateResult result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}
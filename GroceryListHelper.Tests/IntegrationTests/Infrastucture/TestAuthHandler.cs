using GroceryListHelper.Shared.Models.Authentication;

namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

public sealed class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public static Guid UserId { get; } = Guid.NewGuid();
    public const string UserEmail = "test_user@email.com";
    public const string RandomEmail1 = "test1@email.com";
    public const string RandomEmail2 = "test2@email.com";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims =
        [
            new Claim(AuthenticationConstants.NameClaimName, "Test user"),
            new Claim(AuthenticationConstants.EmailClaimName, UserEmail),
            new Claim(AuthenticationConstants.IdClaimName, UserId.ToString())
        ];
        ClaimsIdentity identity = new(claims, "FakeAuth", AuthenticationConstants.EmailClaimName, "role");
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, "FakeAuth");
        AuthenticateResult result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}
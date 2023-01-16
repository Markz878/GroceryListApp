//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Security.Claims;
//using System.Text.Encodings.Web;

//namespace E2ETests;

//public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//{
//    public static Guid UserId { get; } = Guid.NewGuid();
//    public static string Email { get; } = "test_user@email.com";
//    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
//        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
//        : base(options, logger, encoder, clock)
//    {
//    }

//    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//    {
//        Claim[] claims = new[]
//        {
//            new Claim("name", "Test user"),
//            new Claim("preferred_username", Email),
//            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", UserId.ToString())
//        };
//        ClaimsIdentity identity = new(claims, "FakeAuth", "preferred_username", "role");
//        ClaimsPrincipal principal = new(identity);
//        AuthenticationTicket ticket = new(principal, "FakeAuth");

//        AuthenticateResult result = AuthenticateResult.Success(ticket);

//        return Task.FromResult(result);
//    }
//}
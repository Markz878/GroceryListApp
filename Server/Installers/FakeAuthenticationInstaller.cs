namespace GroceryListHelper.Server.Installers;

public class FakeAuthenticationInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddScoped<FakeAuthenticationMiddleware>();
        }
    }
}

internal class FakeAuthenticationMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("fake-username", out StringValues fakeName) && context.Request.Headers.TryGetValue("fake-email", out StringValues fakeEmail) && context.Request.Headers.TryGetValue("fake-userid", out StringValues fakeUserId))
        {
            Claim[] claims = new[]
            {
                new Claim("name", fakeName.ToString()),
                new Claim("preferred_username", fakeEmail.ToString()),
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", fakeUserId.ToString())
            };
            ClaimsIdentity identity = new(claims, "FakeAuth", "preferred_username", "role");
            ClaimsPrincipal principal = new(identity);
            context.User = principal;
        }
        return next(context);
    }
}

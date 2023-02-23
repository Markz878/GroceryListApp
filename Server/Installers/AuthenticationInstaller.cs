using Microsoft.AspNetCore.Authentication.Cookies;

namespace GroceryListHelper.Server.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        builder.Services.Configure(CookieAuthenticationDefaults.AuthenticationScheme, (CookieAuthenticationOptions options) =>
        {
            options.Events.OnSignedIn = async context =>
            {
                IUserRepository userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                ArgumentNullException.ThrowIfNull(context.Principal);
                string? email = context.Principal.GetUserEmail();
                ArgumentNullException.ThrowIfNull(email);
                Guid? userId = context.Principal.GetUserId();
                ArgumentNullException.ThrowIfNull(userId);
                string? name = context.Principal.GetUserName();
                await userRepository.AddUser(email, userId.Value, name);
            };
        });
    }
}

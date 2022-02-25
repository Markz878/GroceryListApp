using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace GroceryListHelper.Server.Installers;

public class AuthenticationInstaller : IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddMicrosoftIdentityWebAppAuthentication(configuration);
        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnSignedIn = context =>
            {
                return Task.CompletedTask;
            };
        });
        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            };
        });
    }
}

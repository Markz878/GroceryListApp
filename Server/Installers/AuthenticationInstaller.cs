using Microsoft.Identity.Web;

namespace GroceryListHelper.Server.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

        //builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        //{
        //    options.Events.OnSignedIn = context =>
        //    {
        //        return Task.CompletedTask;
        //    };
        //});
        //builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        //{
        //    options.Events.OnTokenValidated = context =>
        //    {
        //        return Task.CompletedTask;
        //    };
        //});
    }
}

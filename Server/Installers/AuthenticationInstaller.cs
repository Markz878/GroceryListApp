using Microsoft.Identity.Web;

namespace GroceryListHelper.Server.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
    }
}

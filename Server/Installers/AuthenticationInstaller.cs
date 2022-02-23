using Microsoft.Identity.Web;

namespace GroceryListHelper.Server.Installers;

public class AuthenticationInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebAppAuthentication(configuration, subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true);
    }
}

using Microsoft.AspNetCore.ResponseCompression;

namespace GroceryListHelper.Server.Installers;

public class CartHubInstaller : IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSignalR();//.AddAzureSignalR();
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });
    }
}

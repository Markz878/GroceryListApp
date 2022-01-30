using GroceryListHelper.Server.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

namespace GroceryListHelper.Server.Installers;

public class CartHubInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });
        services.AddSingleton<ICartHubService, CartHubService>();
    }
}

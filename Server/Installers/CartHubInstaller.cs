using GroceryListHelper.Server.Services;
using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.ResponseCompression;

namespace GroceryListHelper.Server.Installers;

public class CartHubInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();//.AddAzureSignalR();
        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });
        builder.Services.AddScoped<ICartHubBuilder, HostCartHubBuilder>();
    }
}

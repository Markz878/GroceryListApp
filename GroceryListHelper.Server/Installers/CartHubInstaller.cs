namespace GroceryListHelper.Server.Installers;

public sealed class CartHubInstaller : IInstaller
{
    private const string octetStream = "application/octet-stream";

    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSignalR(o => o.EnableDetailedErrors = true).AddMessagePackProtocol();
        }
        else
        {
            builder.Services.AddSignalR().AddMessagePackProtocol().AddAzureSignalR();
            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Append(octetStream);
            });
        }

    }
}

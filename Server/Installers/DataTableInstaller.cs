using Azure.Data.Tables;

namespace GroceryListHelper.Server.Installers;

public class DataTableInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment())
        {
            ManagedIdentityCredential credential = new();
            builder.Services.AddSingleton(new TableServiceClient(new Uri($"{builder.Configuration["TableStorageUri"] ?? throw new ArgumentNullException("TableStorageUri configuration value")}"), credential));
        }
        else
        {
            builder.Services.AddSingleton(new TableServiceClient("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"));
        }
    }
}

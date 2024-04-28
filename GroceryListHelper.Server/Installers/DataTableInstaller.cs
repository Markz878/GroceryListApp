using Azure.Data.Tables;

namespace GroceryListHelper.Server.Installers;

public class DataTableInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));
        }
        else
        {
            builder.Services.AddSingleton(new TableServiceClient(new Uri($"{builder.Configuration["TableStorageUri"] ?? throw new ArgumentNullException("TableStorageUri configuration value")}"), new ManagedIdentityCredential()));
        }
    }
}

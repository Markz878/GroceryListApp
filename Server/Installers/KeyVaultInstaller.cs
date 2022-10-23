namespace GroceryListHelper.Server.Installers;

public class KeyVaultInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"), new DefaultAzureCredential());
        }
    }
}

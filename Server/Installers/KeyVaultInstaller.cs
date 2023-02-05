namespace GroceryListHelper.Server.Installers;

public sealed class KeyVaultInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"), new ManagedIdentityCredential());
        }
    }
}

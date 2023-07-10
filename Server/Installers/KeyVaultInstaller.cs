namespace GroceryListHelper.Server.Installers;

public sealed class KeyVaultInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"] ?? throw new ArgumentException("Key vault Uri")), new ManagedIdentityCredential());
        }
    }
}

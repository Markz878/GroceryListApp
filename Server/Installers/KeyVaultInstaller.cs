using Azure.Identity;

namespace GroceryListHelper.Server.Installers;

public class KeyVaultInstaller : IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration)
    {
        configuration.AddAzureKeyVault(new Uri($"https://{configuration["KeyVaultName"]}.vault.azure.net/"), new DefaultAzureCredential());
    }
}

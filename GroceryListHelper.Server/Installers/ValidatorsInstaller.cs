using GroceryListHelper.Core;

namespace GroceryListHelper.Server.Installers;

public sealed class ValidatorsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services
            .AddValidatorsFromAssemblyContaining<CoreMarker>(ServiceLifetime.Singleton)
            .AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
    }
}

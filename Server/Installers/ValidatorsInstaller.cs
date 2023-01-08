namespace GroceryListHelper.Server.Installers;

public sealed class ValidatorsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
    }
}

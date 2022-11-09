namespace GroceryListHelper.Server.Installers;

public class ValidatorsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    }
}

using FluentValidation.AspNetCore;

namespace GroceryListHelper.Server.Installers;

public class ValidatorsInstaller : IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddFluentValidation(config => config.RegisterValidatorsFromAssembly(typeof(Program).Assembly));
    }
}

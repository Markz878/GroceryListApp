using FluentValidation.AspNetCore;

namespace GroceryListHelper.Server.Installers;

public class ValidatorsInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidation(config => config.RegisterValidatorsFromAssembly(typeof(Program).Assembly));
    }
}

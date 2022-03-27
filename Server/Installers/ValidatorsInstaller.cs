using FluentValidation.AspNetCore;

namespace GroceryListHelper.Server.Installers;

public class ValidatorsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddFluentValidation(config => config.RegisterValidatorsFromAssembly(typeof(Program).Assembly));
    }
}

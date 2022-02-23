using FluentValidation.AspNetCore;

namespace GroceryListHelper.Server.Installers;

public class ValidatorsInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        //services.AddTransient<IValidator<CartProduct>, CartProductValidator>();
        //services.AddTransient<IValidator<StoreProductModel>, StoreProductValidator>();
        //services.AddTransient<IValidator<RegisterRequestModel>, RegisterRequestValidator>();
        //services.AddTransient<IValidator<UserCredentialsModel>, UserCredentialValidator>();
        services.AddFluentValidation(config => config.RegisterValidatorsFromAssembly(typeof(Program).Assembly));
    }
}

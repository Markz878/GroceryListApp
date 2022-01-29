using FluentValidation;
using GroceryListHelper.Server.Validators;
using GroceryListHelper.Shared.Models.Authentication;
using GroceryListHelper.Shared.Models.CartProduct;
using GroceryListHelper.Shared.Models.StoreProduct;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.Server.Installers;

public class ValidatorsInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IValidator<CartProduct>, CartProductValidator>();
        services.AddTransient<IValidator<StoreProductModel>, StoreProductValidator>();
        services.AddTransient<IValidator<RegisterRequestModel>, RegisterRequestValidator>();
        services.AddTransient<IValidator<UserCredentialsModel>, UserCredentialValidator>();
    }
}

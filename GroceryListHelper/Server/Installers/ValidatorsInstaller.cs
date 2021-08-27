using FluentValidation;
using GroceryListHelper.Server.Validators;
using GroceryListHelper.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.Server.Installers
{
    public class ValidatorsInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IValidator<CartProduct>, CartProductValidator>();
            services.AddTransient<IValidator<StoreProduct>, StoreProductValidator>();
            services.AddTransient<IValidator<RegisterRequestModel>, RegisterRequestValidator>();
        }
    }
}

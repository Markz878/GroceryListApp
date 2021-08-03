using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.Server.Installers
{
    public class ValidatorsInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<CartProductValidator>();
            services.AddTransient<StoreProductValidator>();
            services.AddTransient(x => new RegisterRequestValidator(x.GetRequiredService<UserRepository>()));
        }
    }
}

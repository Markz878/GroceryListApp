using GroceryListHelper.Server.HelperMethods;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryListHelper.Server.Installers
{
    public class AuthenticationInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            TokenValidationParametersFactory tokenValidationParametersFactory = new(configuration);
            services.AddSingleton(tokenValidationParametersFactory);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParametersFactory.CreateParameters("AccessTokenKey");
            });
            services.AddScoped<JWTAuthenticationManager>();
        }
    }
}

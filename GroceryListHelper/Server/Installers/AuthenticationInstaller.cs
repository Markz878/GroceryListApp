using GroceryListHelper.Server.HelperMethods;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

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
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        PathString path = context.HttpContext.Request.Path;

                        if (path.StartsWithSegments("/carthub"))
                        {
                            string accessToken = context.Request.Query["access_token"];
                            if(string.IsNullOrEmpty(accessToken) && context.Request.Headers.TryGetValue("Authorization", out StringValues token))
                            {
                                accessToken = token;
                            }
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken.Replace("Bearer ", "");
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddScoped<JWTAuthenticationManager>();
        }
    }
}

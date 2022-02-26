using Microsoft.OpenApi.Models;

namespace GroceryListHelper.Server.Installers;

public class SwaggerInstaller : IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroceryListHelper", Version = "v1" });
        });
    }
}

using GroceryListHelper.Server.Filters;

namespace GroceryListHelper.Server.Installers;

public sealed class SwaggerInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.ParameterFilter<FluentValidationParameterFilter>();
            c.RequestBodyFilter<FluentValidationSwaggerFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroceryListHelper", Version = "v1" });
        });
    }
}

namespace GroceryListHelper.Server.Installers;

public sealed class SwaggerInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.RequestBodyFilter<FluentValidationSwaggerFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroceryListHelper", Version = "v1" });
        });
    }
}

namespace GroceryListHelper.Server.Installers;

public class SwaggerInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroceryListHelper", Version = "v1" });
        });
    }
}

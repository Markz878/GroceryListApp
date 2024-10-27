
namespace GroceryListHelper.Server.Installers;

public class CorsInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(x => x.AddDefaultPolicy(p =>
            {
                p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }));
        }
    }
}

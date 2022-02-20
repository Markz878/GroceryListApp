using AspNetCoreRateLimit;
using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Server.Hubs;
using GroceryListHelper.Server.Installers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InstallAssemblyServices(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

app.UseResponseCompression();
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-local-development");
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroceryListHelper.Server v1"));
    app.UseMiddleware<TimerMiddleware>();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.EnsureDatabaseCreated();

app.MapControllers();
app.MapHub<CartHub>("carthub");
app.MapFallbackToFile("index.html");
app.MapHealthChecks("/health");
app.Run();

namespace GroceryListHelper.Server
{
    public partial class Program { }
}

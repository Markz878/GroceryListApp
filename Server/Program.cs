using AspNetCoreRateLimit;
using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.Server.Hubs;
using GroceryListHelper.Server.Installers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.UI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.InstallAssemblyServices(builder.Configuration);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews();// Swagger can't handle AntiForgeryToken validation
}
else
{
    builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
}
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

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
app.MapRazorPages();
app.MapControllers();
//app.UseAzureSignalR(routes =>
//{
//    routes.MapHub<CartHub>("/carthub");
//});
app.MapHub<CartHub>("/carthub");
app.MapFallbackToPage("/_Host");
app.MapHealthChecks("/health");
app.Run();

namespace GroceryListHelper.Server
{
    public partial class Program { }
}

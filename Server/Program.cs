global using AspNetCoreRateLimit;
global using FluentValidation;
global using GroceryListHelper.DataAccess.HelperMethods;
global using GroceryListHelper.DataAccess.Repositories;
global using GroceryListHelper.Server.HelperMethods;
global using GroceryListHelper.Server.Hubs;
global using GroceryListHelper.Server.Installers;
global using GroceryListHelper.Shared.Models.CartProduct;
global using GroceryListHelper.Shared.Models.StoreProduct;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Identity.Web.UI;
global using System.Security.Claims;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.InstallAssemblyServices();
builder.Services.AddControllersWithViews(options => options.Filters.Add(new ServiceExceptionFilter()));

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
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={60 * 60 * 24 * 7}");
    }
});
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

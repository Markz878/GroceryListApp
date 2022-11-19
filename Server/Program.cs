global using Azure.Identity;
global using FluentValidation;
global using FluentValidation.Results;
global using GroceryListHelper.DataAccess.HelperMethods;
global using GroceryListHelper.DataAccess.Repositories;
global using GroceryListHelper.Server.HelperMethods;
global using GroceryListHelper.Server.Hubs;
global using GroceryListHelper.Server.Installers;
global using GroceryListHelper.Server.Services;
global using GroceryListHelper.Shared.Interfaces;
global using GroceryListHelper.Shared.Models.Authentication;
global using GroceryListHelper.Shared.Models.BaseModels;
global using GroceryListHelper.Shared.Models.CartProduct;
global using GroceryListHelper.Shared.Models.RenderLocation;
global using GroceryListHelper.Shared.Models.StoreProduct;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.ResponseCompression;
global using Microsoft.Azure.Cosmos;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Primitives;
global using Microsoft.Identity.Web.UI;
global using Microsoft.OpenApi.Models;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Security.Claims;
global using GroceryListHelper.Server.Endpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.InstallAssemblyServices();
builder.Services.AddSingleton<RenderLocation, ServerRenderedLocation>();

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
app.UseRateLimiter();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.EnsureDatabaseCreated();
app.MapRazorPages();
app.MapAPIEndpoints();
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

global using Azure.Identity;
global using FluentValidation;
global using FluentValidation.Results;
global using GroceryListHelper.Core.Errors;
global using GroceryListHelper.Core.Features.CartProducts;
global using GroceryListHelper.Core.Features.StoreProducts;
global using GroceryListHelper.Core.HelperMethods;
global using GroceryListHelper.Server.Endpoints;
global using GroceryListHelper.Server.Filters;
global using GroceryListHelper.Server.HelperMethods;
global using GroceryListHelper.Server.Hubs;
global using GroceryListHelper.Server.Installers;
global using GroceryListHelper.Server.Pages;
global using GroceryListHelper.Shared.HelperMethods;
global using GroceryListHelper.Shared.Models.Authentication;
global using GroceryListHelper.Shared.Models.CartGroups;
global using GroceryListHelper.Shared.Models.CartProducts;
global using GroceryListHelper.Shared.Models.StoreProducts;
global using MediatR;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.ResponseCompression;
global using Microsoft.Extensions.Primitives;
global using Microsoft.Identity.Web;
global using Microsoft.OpenApi.Models;
global using System.ComponentModel;
global using System.Security.Claims;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.InstallAssemblyServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroceryListHelper.Server v1"));
    app.UseMiddleware<FakeAuthenticationMiddleware>();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600000")
});
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseRateLimiter();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.MapAPIEndpoints();
app.MapHub<CartHub>("/carthub", options => options.AllowStatefulReconnects = true);
app.MapHealthChecks("/health");
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GroceryListHelper.Client._Imports).Assembly);
app.Services.InitDatabase();
app.Run();

namespace GroceryListHelper.Server
{
    public partial class Program { }
}
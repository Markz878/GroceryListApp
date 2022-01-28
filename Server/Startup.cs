using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using GroceryListHelper.Server.Hubs;
using GroceryListHelper.Server.Installers;
using GroceryListHelper.Shared.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace GroceryListHelper.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.InstallAssemblyServices(Configuration);
        services.AddControllers().AddFluentValidation(options =>
        {
            options.RegisterValidatorsFromAssemblyContaining<UserCredentialsValidator>();
        });
        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseResponseCompression();

        if (env.IsDevelopment())
        {
            app.UseExceptionHandler("/error-local-development");
            app.UseWebAssemblyDebugging();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroceryListHelper.Server v1"));
        }
        else
        {
            app.UseExceptionHandler("/error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        if (!InDocker)
        {
            app.UseHttpsRedirection();
        }
        app.UseIpRateLimiting();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<CartHub>("/carthub");
            endpoints.MapFallbackToFile("index.html");
        });
    }

    private static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}

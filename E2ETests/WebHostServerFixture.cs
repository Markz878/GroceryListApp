using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace E2ETests;

public abstract class WebHostServerFixture : IDisposable
{
    private readonly Lazy<Uri> _rootUriInitializer;

    public Uri RootUri => _rootUriInitializer.Value;
    public IHost Host { get; set; }

    public WebHostServerFixture()
    {
        _rootUriInitializer = new Lazy<Uri>(() => new Uri(StartAndGetRootUri()));
    }

    protected static void RunInBackgroundThread(Action action)
    {
        using ManualResetEvent isDone = new ManualResetEvent(false);

        ExceptionDispatchInfo edi = null;
        new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                edi = ExceptionDispatchInfo.Capture(ex);
            }

            isDone.Set();
        }).Start();

        if (!isDone.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new TimeoutException("Timed out waiting for: " + action);
        }

        if (edi != null)
        {
            throw edi.SourceException;
        }
    }

    protected virtual string StartAndGetRootUri()
    {
        // As the port is generated automatically, we can use IServerAddressesFeature to get the actual server URL
        Host = CreateWebHost();
        RunInBackgroundThread(Host.Start);
        return Host.Services.GetRequiredService<IServer>().Features
            .Get<IServerAddressesFeature>()
            .Addresses.Single();
    }

    public virtual void Dispose()
    {
        Host?.Dispose();
        Host?.StopAsync();
    }

    protected abstract IHost CreateWebHost();
}

// ASP.NET Core with a Startup class (MVC / Pages / Blazor Server)
public class WebHostServerFixture<TStartup> : WebHostServerFixture
    where TStartup : class
{
    protected override IHost CreateWebHost()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                // Make UseStaticWebAssets work
                string applicationPath = typeof(TStartup).Assembly.Location;
                string applicationDirectory = Path.GetDirectoryName(applicationPath);

                // In ASP.NET 5, the file is named app.staticwebassets.xml
                // In ASP.NET 6, the file is named app.staticwebassets.runtime.json
#if NET6_0_OR_GREATER
                string name = Path.ChangeExtension(applicationPath, ".staticwebassets.runtime.json");
#else
                var name = Path.ChangeExtension(applicationPath, ".StaticWebAssets.xml");
#endif
                Dictionary<string, string> inMemoryConfiguration = new Dictionary<string, string>
                {
                    //{ WebHostDefaults.StaticWebAssetsKey, name },
                    { "ConnectionStrings:DatabaseConnection", "Data Source=database.db;" },
                    { "RefreshTokenKey", "qwertyuiopasdfghjkl" },
                    { "AccessTokenKey", "qwertyuiopasdfghjkl" }
                };
                config.AddInMemoryCollection(inMemoryConfiguration);
            })
            .ConfigureWebHost(webHostBuilder => webHostBuilder
                .UseKestrel()
                .UseSolutionRelativeContentRoot("Server")
                .UseStaticWebAssets()
                .UseStartup<TStartup>()
                .UseUrls($"http://127.0.0.1:0")) // :0 allows to choose a port automatically
            .Build();
    }
}

// If you are using a Blazor WebAssembly application without a server, you can use the following type.
// TProgram correspond to a type (often `Program`) from the WebAssembly application.
public class BlazorWebAssemblyWebHostFixture<TProgram> : WebHostServerFixture
{
    protected override IHost CreateWebHost()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                // Make UseStaticWebAssets work
                string applicationPath = typeof(TProgram).Assembly.Location;
                string applicationDirectory = Path.GetDirectoryName(applicationPath);

                // In ASP.NET 5, the file is named app.staticwebassets.xml
                // In ASP.NET 6, the file is named app.staticwebassets.runtime.json
#if NET6_0_OR_GREATER
                string name = Path.ChangeExtension(applicationPath, ".staticwebassets.runtime.json");
#else
                var name = Path.ChangeExtension(applicationPath, ".StaticWebAssets.xml");
#endif
                Dictionary<string, string> inMemoryConfiguration = new Dictionary<string, string>
                {
                    [WebHostDefaults.StaticWebAssetsKey] = name,
                };
            })
            .ConfigureWebHost(webHostBuilder => webHostBuilder
                .UseKestrel()
                .UseSolutionRelativeContentRoot(typeof(TProgram).Assembly.GetName().Name)
                .UseStaticWebAssets()
                .UseStartup<Startup>()
                .UseUrls($"http://127.0.0.1:0")) // :0 allows to choose a port automatically
            .Build();
    }

    private sealed class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}

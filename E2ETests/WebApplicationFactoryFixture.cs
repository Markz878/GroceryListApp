using GroceryListHelper.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace E2ETests;

public class WebApplicationFactoryFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls("https://localhost:5001");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost fixtureHost = builder.Build();
        builder.ConfigureWebHost(b => b.UseKestrel());
        IHost host = builder.Build();
        host.Start();
        return fixtureHost;
    }
}

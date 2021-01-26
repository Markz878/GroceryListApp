using GroceryListHelper.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GroceryListHelper.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton<IAccessTokenProvider, CustomAccessTokenProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<BearerTokenHandler>();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddHttpClient("AnonymousClient", client =>
            {
                client.DefaultVersionPolicy = System.Net.Http.HttpVersionPolicy.RequestVersionExact;
                client.DefaultRequestVersion = HttpVersion.Version20;
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            });
            builder.Services.AddHttpClient("ProtectedClient", client =>
            {
                client.DefaultVersionPolicy = System.Net.Http.HttpVersionPolicy.RequestVersionExact;
                client.DefaultRequestVersion = HttpVersion.Version20;
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            })
            .AddHttpMessageHandler<BearerTokenHandler>();

            builder.Services.AddScoped<AuthenticationService>();
            builder.Services.AddScoped<CartProductsService>();
            builder.Services.AddScoped<StoreProductsService>();
            builder.Services.AddScoped<ProfileService>();

            await builder.Build().RunAsync();
        }
    }
}

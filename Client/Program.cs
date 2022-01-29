using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Client.HelperMethods;

namespace GroceryListHelper.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
#if (!DEBUG)
{
        builder.Logging.ClearProviders();
}
#endif
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<ProtectedClientAuthorizationHandler>();

        AsyncCircuitBreakerPolicy<HttpResponseMessage> ciruitBreakerPolicy = HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(15));
        AsyncPolicyWrap<HttpResponseMessage> retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, t => TimeSpan.FromSeconds(2 * t + 2)).WrapAsync(ciruitBreakerPolicy);
        AsyncPolicyWrap<HttpResponseMessage> pollyPolicy = HttpPolicyExtensions.HandleTransientHttpError().FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError) { Content = new StringContent("Sorry, we are experiencing issues now, come back later.") }).WrapAsync(retryPolicy).WrapAsync(ciruitBreakerPolicy);

        builder.Services.AddHttpClient("AnonymousClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddPolicyHandler(pollyPolicy); ;
        builder.Services.AddHttpClient("ProtectedClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
        .AddHttpMessageHandler<ProtectedClientAuthorizationHandler>().AddPolicyHandler(pollyPolicy); ;

        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddScoped<ProfileService>();
        builder.Services.AddScoped<CartHubBuilder>();
        builder.Services.AddScoped<ICartProductsService, CartProductsServiceProvider>();
        builder.Services.AddScoped<IStoreProductsService, StoreProductsServiceProvider>();
        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddSingleton<IndexViewModel>();
        builder.Services.AddSingleton<ModalViewModel>();

        await builder.Build().RunAsync();
    }
}

using Blazored.LocalStorage;
using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Wrap;

namespace GroceryListHelper.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
#if (!DEBUG)
        builder.Logging.ClearProviders();
#endif
        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
        builder.Services.AddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
        builder.Services.AddTransient<AuthorizedHandler>();

        AsyncCircuitBreakerPolicy<HttpResponseMessage> ciruitBreakerPolicy = HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(15));
        AsyncPolicyWrap<HttpResponseMessage> retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, t => TimeSpan.FromSeconds(2 * t + 2)).WrapAsync(ciruitBreakerPolicy);
        AsyncPolicyWrap<HttpResponseMessage> pollyPolicy = HttpPolicyExtensions.HandleTransientHttpError().FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError) { Content = new StringContent("Sorry, we are experiencing issues now, come back later.") }).WrapAsync(retryPolicy).WrapAsync(ciruitBreakerPolicy);

        builder.Services.AddHttpClient("AnonymousClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddPolicyHandler(pollyPolicy); ;
        builder.Services.AddHttpClient("ProtectedClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
        .AddHttpMessageHandler<AuthorizedHandler>().AddPolicyHandler(pollyPolicy); ;

        builder.Services.AddScoped<CartHubBuilder>();
        builder.Services.AddScoped<ICartProductsService, CartProductsServiceProvider>();
        builder.Services.AddScoped<IStoreProductsService, StoreProductsServiceProvider>();
        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddScoped<IndexViewModel>();
        builder.Services.AddScoped<ModalViewModel>();

        await builder.Build().RunAsync();
    }
}

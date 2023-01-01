global using Blazored.LocalStorage;
global using FluentValidation;
global using FluentValidation.Results;
global using GroceryListHelper.Client.Authentication;
global using GroceryListHelper.Client.HelperMethods;
global using GroceryListHelper.Client.Services;
global using GroceryListHelper.Client.Validators;
global using GroceryListHelper.Client.ViewModels;
global using GroceryListHelper.Shared.Interfaces;
global using GroceryListHelper.Shared.Models.Authentication;
global using GroceryListHelper.Shared.Models.BaseModels;
global using GroceryListHelper.Shared.Models.CartProduct;
global using GroceryListHelper.Shared.Models.RenderLocation;
global using GroceryListHelper.Shared.Models.StoreProduct;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.JSInterop;
global using Polly;
global using Polly.CircuitBreaker;
global using Polly.Extensions.Http;
global using Polly.Retry;
global using Polly.Wrap;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Reflection;
global using System.Security.Claims;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>();
builder.Services.AddTransient<AuthorizedHandler>();

AsyncCircuitBreakerPolicy<HttpResponseMessage> ciruitBreakerPolicy = HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(15));
AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, t => TimeSpan.FromSeconds(2 * t + 2));
AsyncPolicyWrap<HttpResponseMessage> pollyPolicy = HttpPolicyExtensions.HandleTransientHttpError().FallbackAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("Sorry, we are experiencing issues now, come back later.") }).WrapAsync(retryPolicy).WrapAsync(ciruitBreakerPolicy);

builder.Services.AddHttpClient("AnonymousClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddPolicyHandler(pollyPolicy); ;
builder.Services.AddHttpClient("ProtectedClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthorizedHandler>().AddPolicyHandler(pollyPolicy);

builder.Services.AddScoped<ICartHubBuilder, CartHubBuilder>();
builder.Services.AddScoped<ICartProductsService, CartProductsServiceProvider>();
builder.Services.AddScoped<IStoreProductsService, StoreProductsServiceProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<RenderLocation, ClientRenderLocation>();

builder.Services.AddScoped<IndexViewModel>();
builder.Services.AddScoped<ModalViewModel>();

await builder.Build().RunAsync();

global using Blazored.LocalStorage;
global using FluentValidation;
global using GroceryListHelper.Client.Authentication;
global using GroceryListHelper.Client.Interfaces;
global using GroceryListHelper.Shared.HelperMethods;
global using GroceryListHelper.Shared.Models.Authentication;
global using GroceryListHelper.Shared.Models.CartProducts;
global using GroceryListHelper.Shared.Models.HelperModels;
global using GroceryListHelper.Shared.Models.StoreProducts;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
global using System.ComponentModel;
global using System.Net.Http.Json;
global using GroceryListHelper.Client.HelperMethods;
global using GroceryListHelper.Client.Services;
global using GroceryListHelper.Shared.Interfaces;
global using MediatR;
using GroceryListHelper.Client.Features.CartProducts;
using GroceryListHelper.Client.Features.StoreProducts;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AntiforgeryTokenHandler>();
builder.Services.AddHttpClient("Client", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AntiforgeryTokenHandler>()
    .AddStandardResilienceHandler();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<RenderLocation, ClientRenderLocation>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<CartProductsServiceProvider>();
builder.Services.AddScoped<StoreProductsServiceProvider>();
builder.Services.AddKeyedScoped<ICartProductsService, CartProductsLocalService>(ServiceKey.Local);
builder.Services.AddKeyedScoped<ICartProductsService, CartProductsApiService>(ServiceKey.Api);
builder.Services.AddKeyedScoped<ICartProductsService, CartProductsGroupService>(ServiceKey.Group);
builder.Services.AddKeyedScoped<IStoreProductsService, StoreProductsLocalService>(ServiceKey.Local);
builder.Services.AddKeyedScoped<IStoreProductsService, StoreProductsAPIService>(ServiceKey.Api);
builder.Services.AddScoped<ICartHubClient, CartHubClient>(sp =>
{
    AppState appState = sp.GetRequiredService<AppState>();
    NavigationManager navigation = sp.GetRequiredService<NavigationManager>();
    return new CartHubClient(appState, navigation.ToAbsoluteUri("/carthub"));
});
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());
await builder.Build().RunAsync();

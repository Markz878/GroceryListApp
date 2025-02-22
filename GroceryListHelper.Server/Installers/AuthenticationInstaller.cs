﻿using GroceryListHelper.Core.Features.Users;
using GroceryListHelper.Server.HelperMethods;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace GroceryListHelper.Server.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddAntiforgery(options => { options.HeaderName = "X-XSRF-TOKEN"; options.Cookie.SameSite = SameSiteMode.Strict; });
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
        builder.Services.Configure(CookieAuthenticationDefaults.AuthenticationScheme, (CookieAuthenticationOptions options) =>
        {
            options.Events.OnSignedIn = async context =>
            {
                try
                {
                    ArgumentNullException.ThrowIfNull(context.Principal);
                    string? email = context.Principal.GetUserEmail();
                    ArgumentNullException.ThrowIfNull(email);
                    string? name = context.Principal.GetUserName();
                    IMediator mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
                    await mediator.Send(new AddUserCommand() { Email = email, Name = name ?? "" });
                }
                catch (Exception ex)
                {
                    ILogger<AuthenticationInstaller> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationInstaller>>();
                    logger.LogError(ex, "Could not upsert user.");
                    throw;
                }

            };
        });
        builder.Services.Configure(OpenIdConnectDefaults.AuthenticationScheme, (OpenIdConnectOptions options) =>
        {
            options.Events.OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.Prompt = "select_account";
                return Task.CompletedTask;
            };
        });
    }
}

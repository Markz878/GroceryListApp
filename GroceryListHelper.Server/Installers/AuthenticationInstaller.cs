using GroceryListHelper.Core.Features.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace GroceryListHelper.Server.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddAntiforgery(options => { options.HeaderName = "X-XSRF-TOKEN"; });
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
        builder.Services.Configure(CookieAuthenticationDefaults.AuthenticationScheme, (CookieAuthenticationOptions options) =>
        {
            options.Events.OnSignedIn = async context =>
            {
                ArgumentNullException.ThrowIfNull(context.Principal);
                string? email = context.Principal.GetUserEmail();
                ArgumentNullException.ThrowIfNull(email);
                Guid? userId = context.Principal.GetUserId();
                ArgumentNullException.ThrowIfNull(userId);
                string? name = context.Principal.GetUserName();
                IMediator mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
                await mediator.Send(new AddUserCommand() { Email = email, Id = userId.Value, Name = name ?? "" });
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

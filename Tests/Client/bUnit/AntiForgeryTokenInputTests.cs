using Bunit;
using GroceryListHelper.Client.Components;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Tests.Client.bUnit;
public class AntiForgeryTokenInputTests : TestContext
{
    [Fact]
    public void AntiForgeryToken_Success()
    {
        const string token = "QWERTY";
        JSInterop.Setup<string>("getAntiForgeryToken").SetResult(token);
        Services.AddSingleton<RenderLocation>(new ClientRenderLocation());
        IRenderedComponent<AntiForgeryTokenInput> cut = RenderComponent<AntiForgeryTokenInput>();
        cut.MarkupMatches($"<input type=\"hidden\" id=\"__RequestVerificationToken\" name=\"__RequestVerificationToken\" value=\"{token}\" />");
        JSInterop.VerifyInvoke("getAntiForgeryToken", 1);
    }

    [Fact]
    public void AntiForgeryToken_ServerRender()
    {
        const string token = "QWERTY";
        JSInterop.Setup<string>("getAntiForgeryToken").SetResult(token);
        Services.AddSingleton<RenderLocation>(new ServerRenderedLocation());
        IRenderedComponent<AntiForgeryTokenInput> cut = RenderComponent<AntiForgeryTokenInput>();
        cut.MarkupMatches("");
        JSInterop.VerifyNotInvoke("getAntiForgeryToken");
    }
}

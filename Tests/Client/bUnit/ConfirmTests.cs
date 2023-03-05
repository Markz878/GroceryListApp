using Bunit;
using GroceryListHelper.Client.Components;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GroceryListHelper.Tests.Client.bUnit;
public class ConfirmTests : TestContext
{
    [Fact]
    public async Task WhenConfirmShown_Success()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./Components/Confirm.razor.js");
        module.SetupVoid("showModal", _ => true).SetVoidResult();
        IRenderedComponent<Confirm> cut = RenderComponent<Confirm>(p => p
            .Add(x => x.Header, "Confirm")
            .Add(x => x.Message, "Are you sure?")
            .Add(x => x.OkCallback, () => { }));
        await cut.Instance.ShowConfirm();
        Assert.Single(module.Invocations);
    }
}

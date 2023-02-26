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
        cut.MarkupMatches("""
            <dialog class="confirm" b-w48gy521tc blazor:elementReference="61a9a175-2f6f-4e0f-811d-70f8bfe6d0fe">
                <div class="modal-content" b-w48gy521tc>
                    <div class="modal-header" b-w48gy521tc>
                        <h3 class="header-text" b-w48gy521tc>Confirm</h3>
                    </div>
                    <div class="modal-body" b-w48gy521tc>
                        <p class="body-text" b-w48gy521tc>Are you sure?</p>
                        <form method="dialog" b-w48gy521tc>
                            <button class="btn btn-success" autofocus blazor:onclick="1" b-w48gy521tc>Ok</button>
                            <button class="btn btn-danger" b-w48gy521tc>Cancel</button>
                        </form>
                    </div>
                </div>
            </dialog>
            """);
    }
}

using Bunit;
using GroceryListHelper.Client.Components;
using GroceryListHelper.Client.ViewModels;

namespace GroceryListHelper.Tests.Client.bUnit;
public class ModalTests : TestContext
{
    [Fact]
    public void WhenModalShown_Success()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./Components/Modal.razor.js");
        module.SetupVoid("showModal", _ => true).SetVoidResult();
        Services.AddSingleton(new ModalViewModel());
        IRenderedComponent<Modal> cut = RenderComponent<Modal>();
        cut.Instance.ViewModel.ShowError("This is an error message");
        Assert.Single(module.Invocations);
        Assert.Contains("<dialog id=\"modal\" class=\"modal\"", cut.Markup);
    }
}

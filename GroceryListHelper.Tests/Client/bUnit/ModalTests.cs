namespace GroceryListHelper.Tests.Client.bUnit;
public class ModalTests : TestContext
{
    [Fact]
    public void WhenModalShown_Success()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./Components/Modal.razor.js");
        module.SetupVoid("showModal", _ => true).SetVoidResult();
        Services.AddCascadingValue(sp => new AppState());
        Services.AddSingleton<RenderLocation>(new ClientRenderLocation());
        IRenderedComponent<Modal> cut = RenderComponent<Modal>();
        cut.Instance.AppState.ShowError("This is an error message");
        Assert.Single(module.Invocations);
        Assert.Contains("<dialog id=\"modal\"", cut.Markup);
    }
}

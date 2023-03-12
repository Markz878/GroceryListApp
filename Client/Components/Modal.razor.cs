namespace GroceryListHelper.Client.Components;

public abstract class ModalBase : BasePage<ModalViewModel>
{
    [Inject] public required IJSRuntime JS { get; set; }
    protected ElementReference modal;
    private IJSObjectReference? module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Modal.razor.js");
            ViewModel.StateChanged += PropertyChanged;
        }
    }

    private async Task PropertyChanged()
    {
        if (!string.IsNullOrWhiteSpace(ViewModel.Header) && !string.IsNullOrWhiteSpace(ViewModel.Message) && module is not null)
        {
            await module.InvokeVoidAsync("showModal", modal);
        }
    }

    public void CloseModal()
    {
        ViewModel.Clear();
    }

    protected string GetHeaderBackgroundClass()
    {
        return ViewModel.Header == "Error" ? "header-error" : "header-info";
    }

    public override void Dispose()
    {
        ViewModel.StateChanged -= PropertyChanged;
        base.Dispose();
    }
}

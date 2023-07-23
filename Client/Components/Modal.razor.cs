namespace GroceryListHelper.Client.Components;

public abstract class ModalBase : BasePage<ModalViewModel>
{
    [Inject] public required IJSRuntime JS { get; set; }
    protected ElementReference modal;
    private IJSObjectReference? module;
    protected string? HeaderBackgroundClass { get; set; }
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
            SetHeaderBackgroundClass();
            await module.InvokeVoidAsync("showModal", modal);
        }
    }

    public void CloseModal()
    {
        ViewModel.Clear();
    }

    protected void SetHeaderBackgroundClass()
    {
        HeaderBackgroundClass = ViewModel.Header == "Error" ? "bg-red-600" : "bg-green-600";
    }

    public override ValueTask DisposeAsync()
    {
        ViewModel.StateChanged -= PropertyChanged;
        return base.DisposeAsync();
    }
}

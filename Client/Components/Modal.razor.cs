namespace GroceryListHelper.Client.Components;

public abstract class ModalBase : BasePage<MainViewModel>
{
    [Inject] public required IJSRuntime JS { get; set; }
    protected string? HeaderBackgroundClass => ViewModel.Header == "Error" ? "bg-red-600" : "bg-green-600";

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
        if (module is not null && !string.IsNullOrWhiteSpace(ViewModel.Header) && !string.IsNullOrWhiteSpace(ViewModel.Message))
        {
            await module.InvokeVoidAsync("showModal", modal);
        }
    }

    public void CloseModal()
    {
        ViewModel.Clear();
    }

    public override ValueTask DisposeAsync()
    {
        ViewModel.StateChanged -= PropertyChanged;
        return base.DisposeAsync();
    }
}

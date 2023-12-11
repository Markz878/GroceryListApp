using Microsoft.JSInterop;

namespace GroceryListHelper.Client.Components;

public sealed partial class Modal : IDisposable
{
    [Inject] public required RenderLocation RenderLocation { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }
    [CascadingParameter] public required AppState AppState { get; init; }

    private string? HeaderBackgroundClass => AppState.Header == "Error" ? "bg-red-600" : "bg-green-600";
    private ElementReference modal;
    private IJSObjectReference? module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (RenderLocation is ClientRenderLocation && firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Modal.razor.js");
            AppState.StateChanged += PropertyChanged;
        }
    }

    private async Task PropertyChanged()
    {
        if (module is not null && !string.IsNullOrWhiteSpace(AppState.Header) && !string.IsNullOrWhiteSpace(AppState.Message))
        {
            await module.InvokeVoidAsync("showModal", modal);
        }
    }

    public void CloseModal()
    {
        AppState.Clear();
    }

    public void Dispose()
    {
        AppState.StateChanged -= PropertyChanged;
    }
}

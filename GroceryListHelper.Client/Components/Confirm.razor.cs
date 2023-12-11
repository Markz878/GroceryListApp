using Microsoft.JSInterop;

namespace GroceryListHelper.Client.Components;

public partial class Confirm
{
    [Parameter][EditorRequired] public required string Header { get; set; }
    [Parameter][EditorRequired] public required string Message { get; set; }
    [Parameter][EditorRequired] public required EventCallback OkCallback { get; set; }
    [Parameter] public EventCallback CancelCallback { get; set; }
    [Inject] public required RenderLocation RenderLocation { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }

    private ElementReference modal;
    private IJSObjectReference? module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (RenderLocation is ClientRenderLocation && firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Confirm.razor.js");
        }
    }

    public async Task ShowConfirm()
    {
        if (module is not null)
        {
            await module.InvokeVoidAsync("showModal", modal);
        }
    }
}

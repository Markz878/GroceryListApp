namespace GroceryListHelper.Client.HelperMethods;

public abstract class BasePage<T> : ComponentBase, IDisposable where T : BaseViewModel
{
    [Inject] public T ViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += PropertyChanged;
    }

    private async Task PropertyChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    public virtual void Dispose()
    {
        ViewModel.StateChanged -= PropertyChanged;
        GC.SuppressFinalize(this);
    }
}

using Microsoft.AspNetCore.Components;
using System;

namespace GroceryListHelper.Client.HelperMethods;

public abstract class BasePage<T> : ComponentBase, IDisposable where T : BaseViewModel
{
    [Inject] public T ViewModel { get; set; }

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += PropertyChanged;
    }

    private void PropertyChanged()
    {
        StateHasChanged();
    }

    public virtual void Dispose()
    {
        ViewModel.StateChanged -= PropertyChanged;
        GC.SuppressFinalize(this);
    }
}

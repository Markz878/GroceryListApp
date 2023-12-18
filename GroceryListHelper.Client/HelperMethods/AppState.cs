using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GroceryListHelper.Client.HelperMethods;

public sealed class AppState : IDisposable
{
    public ObservableCollection<CartProductCollectable> CartProducts { get; } = [];
    public ObservableCollection<StoreProduct> StoreProducts { get; } = [];
    public SortState SortDirection { get => sortDirection; set => SetProperty(ref sortDirection, value); }
    private SortState sortDirection;
    public bool IsSharing { get => isSharing; set => SetProperty(ref isSharing, value); }
    private bool isSharing;
    public bool ShowOnlyUncollected { get => showOnlyUncollected; set => SetProperty(ref showOnlyUncollected, value); }
    private bool showOnlyUncollected;
    public string Header { get => header; private set => SetProperty(ref header, value); }
    private string header = string.Empty;
    public string Message { get => message; private set => SetProperty(ref message, value); }
    private string message = string.Empty;
    public void ShowInfo(string message)
    {
        Header = "Info";
        Message = message;
    }
    public void ShowError(string message)
    {
        Header = "Error";
        Message = message;
    }
    public void Clear()
    {
        Header = string.Empty;
        Message = string.Empty;
    }

    #region ViewModel boilerplate
    public Func<Task>? StateChanged { get; set; }

    public AppState()
    {
        CartProducts.CollectionChanged += CollectionChanged!;
        StoreProducts.CollectionChanged += CollectionChanged!;
    }

    public void OnPropertyChanged()
    {
        StateChanged?.Invoke();
    }

    private void SetProperty<T>(ref T backingFiled, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(backingFiled, value))
        {
            backingFiled = value;
            OnPropertyChanged();
        }
    }

    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    public void Dispose()
    {
        CartProducts.CollectionChanged -= CollectionChanged!;
        StoreProducts.CollectionChanged -= CollectionChanged!;
    }
    #endregion
}

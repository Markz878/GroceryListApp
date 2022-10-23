namespace GroceryListHelper.Client.HelperMethods;

public abstract class BaseViewModel : IDisposable
{
    public event Action? StateChanged;
    public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
    private bool isBusy;

    public BaseViewModel()
    {
        foreach (PropertyInfo observableCollection in GetType().GetProperties().Where(x => typeof(INotifyCollectionChanged).IsAssignableFrom(x.PropertyType)))
        {
            if (observableCollection.GetValue(this) is INotifyCollectionChanged property)
            {
                property.CollectionChanged += CollectionChanged!;
            }
        }
    }

    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    public void OnPropertyChanged()
    {
        StateChanged?.Invoke();
    }

    protected void SetProperty<T>(ref T backingFiled, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(backingFiled, value))
        {
            backingFiled = value;
            OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        foreach (PropertyInfo obsCollection in GetType().GetProperties().Where(x => typeof(INotifyCollectionChanged).IsAssignableFrom(x.PropertyType)))
        {
            (obsCollection.GetValue(this) as INotifyCollectionChanged).CollectionChanged -= CollectionChanged;
        }
        GC.SuppressFinalize(this);
    }
}

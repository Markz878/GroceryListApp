namespace GroceryListHelper.Client.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    public ObservableCollection<CartProductUIModel> CartProducts { get; } = new();
    public ObservableCollection<StoreProduct> StoreProducts { get; } = new();
    public string ShareCartInfo { get => shareCartInfo; set => SetProperty(ref shareCartInfo, value); }
    private string shareCartInfo = string.Empty;
    public bool IsPolling { get => isPolling; set => SetProperty(ref isPolling, value); }
    private bool isPolling;
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
}

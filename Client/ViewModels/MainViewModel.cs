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
}

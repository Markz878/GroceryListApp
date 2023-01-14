namespace GroceryListHelper.Client.ViewModels;

public sealed class IndexViewModel : BaseViewModel
{
    public ObservableCollection<CartProductUIModel> CartProducts { get; } = new();
    public ObservableCollection<StoreProductUIModel> StoreProducts { get; } = new();
    public ObservableCollection<string> AllowedUsers { get; } = new();
    public ShareModeType ShareMode { get => shareMode; set => SetProperty(ref shareMode, value); }
    private ShareModeType shareMode;
    public string CartHostEmail { get => cartHostEmail; set => SetProperty(ref cartHostEmail, value); }
    private string cartHostEmail = string.Empty;
    public string AllowEmail { get => allowEmail; set => SetProperty(ref allowEmail, value); }
    private string allowEmail = string.Empty;
    public string ShareCartInfo { get => shareCartInfo; set => SetProperty(ref shareCartInfo, value); }
    private string shareCartInfo = string.Empty;
    public bool IsPolling { get => isPolling; set => SetProperty(ref isPolling, value); }
    private bool isPolling;
    public bool ShowOnlyUncollected { get => showOnlyUncollected; set => SetProperty(ref showOnlyUncollected, value); }
    private bool showOnlyUncollected;
}

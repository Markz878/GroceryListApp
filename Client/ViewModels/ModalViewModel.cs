namespace GroceryListHelper.Client.ViewModels;

public sealed class ModalViewModel : BaseViewModel
{
    public string Header { get => header; set => SetProperty(ref header, value); }
    private string header = string.Empty;
    public string Message { get => message; set => SetProperty(ref message, value); }
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
        header = string.Empty;
        Message = string.Empty;
    }
}

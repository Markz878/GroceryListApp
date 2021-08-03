using GroceryListHelper.Client.HelperMethods;

namespace GroceryListHelper.Client.ViewModels
{
    public class ModalViewModel : BaseViewModel
    {
        public string Message { get => message; set => SetProperty(ref message, value); }
        private string message;

        public string Header { get => header; set => SetProperty(ref header, value); }
        private string header;
    }
}

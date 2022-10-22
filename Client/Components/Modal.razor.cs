namespace GroceryListHelper.Client.Components;

public class ModalBase : BasePage<ModalViewModel>
{
    public void CloseModal()
    {
        ViewModel.Header = "";
        ViewModel.Message = "";
    }
}

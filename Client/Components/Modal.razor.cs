namespace GroceryListHelper.Client.Components;

public abstract class ModalBase : BasePage<ModalViewModel>
{
    public void CloseModal()
    {
        ViewModel.Header = "";
        ViewModel.Message = "";
    }
}

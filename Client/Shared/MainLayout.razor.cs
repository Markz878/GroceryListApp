namespace GroceryListHelper.Client.Shared;

public partial class MainLayout
{
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    public void GoToIndex()
    {
        Navigation.NavigateTo("/");
    }
}

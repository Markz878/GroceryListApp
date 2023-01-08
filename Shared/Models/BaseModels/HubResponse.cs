namespace GroceryListHelper.Shared.Models.BaseModels;

public sealed class HubResponse
{
    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

using GroceryListHelper.Shared.Models.CartGroups;

namespace GroceryListHelper.Client.Components;
public partial class GroupNameComponent
{
    [Parameter][EditorRequired] public required CartGroup CartGroup { get; init; }
    [CascadingParameter] public required AppState AppState { get; init; }
    [Inject] public required RenderLocation RenderLocation { get; init; }
}
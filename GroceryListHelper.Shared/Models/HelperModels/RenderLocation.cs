namespace GroceryListHelper.Shared.Models.HelperModels;
public abstract class RenderLocation { }
public sealed class ClientRenderLocation : RenderLocation { }
public sealed class ServerRenderedLocation : RenderLocation { }

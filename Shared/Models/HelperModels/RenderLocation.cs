namespace GroceryListHelper.Shared.Models.RenderLocation;
public abstract class RenderLocation { }
public sealed class ClientRenderLocation : RenderLocation { }
public sealed class ServerRenderedLocation : RenderLocation { }

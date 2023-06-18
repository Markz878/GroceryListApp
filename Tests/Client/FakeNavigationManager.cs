using Microsoft.AspNetCore.Components;

namespace GroceryListHelper.Tests.Client;

internal class FakeNavigationManager : NavigationManager
{
    private readonly string baseUri;
    private readonly string uri;
    public FakeNavigationManager(string baseUri, string uri)
    {
        this.baseUri = baseUri;
        this.uri = uri;
    }
    protected override void EnsureInitialized()
    {
        Initialize(baseUri, uri);
    }
}

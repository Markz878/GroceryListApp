using Bunit;
using Bunit.TestDoubles;
using GroceryListHelper.Client.Components;

namespace GroceryListHelper.Tests.Client.bUnit;
public class AuthorizedViewTests : TestContext
{
    [Fact]
    public void AuthorizedViewTest_Success()
    {
        TestAuthorizationContext auth = this.AddTestAuthorization();
        auth.SetAuthorized("Test user");
        this.AddFakePersistentComponentState();
        IRenderedComponent<AuthorizedView> cut = RenderComponent<AuthorizedView>(p => p.Add(x => x.ChildContent, "<h1>Hello</h1>"));
        cut.MarkupMatches("<h1>Hello</h1>");
    }

    [Fact]
    public void AuthorizedViewTest_AuthorizedConditionFailed()
    {
        TestAuthorizationContext auth = this.AddTestAuthorization();
        auth.SetAuthorized("Test user");
        this.AddFakePersistentComponentState();
        IRenderedComponent<AuthorizedView> cut = RenderComponent<AuthorizedView>(p => p
            .Add(x => x.ChildContent, "<h1>Hello</h1>")
            .Add(x => x.AuthorizedCondition, () => Task.FromResult(false)));
        cut.MarkupMatches("""
            <div style="padding: 1rem;">
                <h4>You are not authorized to see this content.</h4>
                <a href="/">Back to App</a>
            </div>
            """);
    }
}

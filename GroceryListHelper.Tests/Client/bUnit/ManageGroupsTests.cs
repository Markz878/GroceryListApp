using Bunit.TestDoubles;
using GroceryListHelper.Client.Features.ManageGroups;
using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Server.Pages;

namespace GroceryListHelper.Tests.Client.bUnit;

public class ManageGroupsTests : TestContext
{
    public ManageGroupsTests()
    {
        BunitJSModuleInterop module1 = JSInterop.SetupModule("./Components/Confirm.razor.js");
        module1.SetupVoid("showModal", _ => true).SetVoidResult();
        BunitJSModuleInterop module2 = JSInterop.SetupModule("./Components/Modal.razor.js");
        module2.SetupVoid("showModal", _ => true).SetVoidResult();
        IMediator mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetUserCartGroupsQuery>()).Returns([]);
        mediator.Send(Arg.Any<CreateGroupClientCommand>()).Returns(new Result<Guid, string>(Guid.NewGuid()));
        Services.AddSingleton(mediator);
        Services.AddSingleton(new AppState());
        Services.AddSingleton<RenderLocation>(new ClientRenderLocation());
    }


    [Fact]
    public void WhenGroupIsCreated_GroupIsShownInList()
    {
        TestAuthorizationContext authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TEST USER");
        IRenderedComponent<ManageGroups> cut = RenderComponent<ManageGroups>();
        cut.Find("#create-group-btn").Click();
        cut.Find("#create-group-name").Change("Test Group");
        cut.Find("#add-user-to-group").Change("test1@email.com");
        cut.Find("#add-member-btn").Click();
        cut.Find("#create-group-btn").Click();
        Assert.Contains("Members: You, test1@email.com", cut.Markup);
    }
}

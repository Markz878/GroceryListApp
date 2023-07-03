using Bunit;
using Bunit.TestDoubles;
using GroceryListHelper.Client.Pages;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.CartGroups;
using Microsoft.JSInterop;
using NSubstitute;

namespace GroceryListHelper.Tests.Client.bUnit;

public class ManageGroupsTests : TestContext
{
    private readonly ICartGroupsService cartGroupsService = Substitute.For<ICartGroupsService>();

    public ManageGroupsTests()
    {
        Services.AddSingleton(new MainViewModel());
        Services.AddSingleton(new ModalViewModel());
        Services.AddSingleton(cartGroupsService);
        this.AddFakePersistentComponentState();
        BunitJSModuleInterop module = JSInterop.SetupModule("./Components/Confirm.razor.js");
        module.SetupVoid("showModal", _ => true).SetVoidResult();
    }

    [Fact]
    public void WhenNotAuthorized_GroupsNotShown()
    {
        TestAuthorizationContext authContext = this.AddTestAuthorization();
        IRenderedComponent<ManageGroups> cut = RenderComponent<ManageGroups>();
        Assert.Contains("You are not authorized", cut.Markup);
    }

    [Fact]
    public void WhenGroupIsCreated_GroupIsShownInList()
    {
        cartGroupsService.GetCartGroups().Returns(new List<CartGroup>());
        cartGroupsService.CreateCartGroup(Arg.Any<CreateCartGroupRequest>()).Returns(new CartGroup() { Name = "Test Group", OtherUsers = new HashSet<string>() { "test1@email.com" } });
        TestAuthorizationContext authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TEST USER");
        IRenderedComponent<ManageGroups> cut = RenderComponent<ManageGroups>();
        cut.Find(".create-btn").Click();
        cut.Find("#create-group-name").Change("Test Group");
        cut.Find("#add-user-to-group").Change("test1@email.com");
        cut.Find(".add-btn").Click();
        cut.Find(".create-btn").Click();
        Assert.Contains("You, test1@email.com", cut.Find("p.group-members").InnerHtml);
    }
}

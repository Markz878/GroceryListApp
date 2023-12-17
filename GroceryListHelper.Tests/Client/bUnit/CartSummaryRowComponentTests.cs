using AngleSharp.Diffing.Extensions;
using GroceryListHelper.Client.Features.CartProducts;
using GroceryListHelper.Client.Features.StoreProducts;

namespace GroceryListHelper.Tests.Client.bUnit;

public class CartSummaryRowComponentTests : TestContext
{
    private readonly AppState vm = new();
    private readonly IMediator mediatorMock = Substitute.For<IMediator>();

    public CartSummaryRowComponentTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./Components/Confirm.razor.js");
        module.SetupVoid("showModal", _ => true).SetVoidResult();
        Services.AddSingleton(vm);
        Services.AddSingleton(mediatorMock);
        Services.AddSingleton<RenderLocation>(new ClientRenderLocation());
        Services.AddCascadingValue(sp => vm);
    }
    public static TheoryData<List<CartProductCollectable>> CartProductListData => new()
    {
        {
            new List<CartProductCollectable>()
            {
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 1", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 2", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000 },
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 3", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 }
            }
        },
        {
            new List<CartProductCollectable>()
            {
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 1", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 2", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000, IsCollected = true },
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 3", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 }
            }
        },
        {
            new List<CartProductCollectable>()
            {
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 1", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000, IsCollected = true },
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 2", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000, IsCollected = true },
                 new() { Amount = Random.Shared.Next(1,5), Name = "Product 3", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000, IsCollected = true }
            }
        },
    };

    [Theory]
    [MemberData(nameof(CartProductListData))]
    public void WhenCartHasItems_ShowsCorrectTotal(List<CartProductCollectable> products)
    {
        vm.CartProducts.AddRange(products);
        IRenderedComponent<CartSummaryRowComponent> cut = RenderComponent<CartSummaryRowComponent>();
        double expectedPrice = Math.Round(vm.CartProducts.Select(x => x.Amount * x.UnitPrice).Sum(), 2);
        Assert.Contains(expectedPrice.ToString("N2", CultureInfo.InvariantCulture), cut.Markup);
    }

    [Theory]
    [MemberData(nameof(CartProductListData))]
    public void WhenCartHasItems_AndClearCartProductsButtonIsPressed_CartIsCleared(List<CartProductCollectable> products)
    {
        vm.CartProducts.AddRange(products);
        IRenderedComponent<CartSummaryRowComponent> cut = RenderComponent<CartSummaryRowComponent>();
        cut.Find("button[aria-label=\"Clear cart\"]").Click();
        cut.Find("button[aria-label=\"Yes\"]").Click();
        Assert.Contains("Total: 0", cut.Markup);
        mediatorMock.Received(1).Send(Arg.Any<DeleteAllCartProductsCommand>());
    }

    [Fact]
    public void WhenStoreProductHasItems_AndClearStoreProductsButtonIsPressed_StoreProductsIsCleared()
    {
        IRenderedComponent<CartSummaryRowComponent> cut = RenderComponent<CartSummaryRowComponent>();
        cut.Find("button[aria-label=\"Clear store products\"]").Click();
        cut.Find("button[aria-label=\"Yes\"]").Click();
        Assert.Empty(vm.StoreProducts);
        mediatorMock.Received(1).Send(Arg.Any<DeleteAllStoreProductsCommand>());
    }
}

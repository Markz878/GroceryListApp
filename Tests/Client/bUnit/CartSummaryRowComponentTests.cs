using AngleSharp.Dom;
using Bunit;
using GroceryListHelper.Client.Components;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using NSubstitute;
using System.Globalization;

namespace GroceryListHelper.Tests.Client.bUnit;

public class CartSummaryRowComponentTests : TestContext
{
    private readonly MainViewModel vm = new();
    private readonly ICartProductsServiceFactory cartProductsServiceFactoryMock = Substitute.For<ICartProductsServiceFactory>();
    private readonly ICartProductsService cartProductsServiceMock = Substitute.For<ICartProductsService>();
    private readonly IStoreProductsServiceFactory storeProductsServiceFactoryMock = Substitute.For<IStoreProductsServiceFactory>();
    private readonly IStoreProductsService storeProductsServiceMock = Substitute.For<IStoreProductsService>();

    public CartSummaryRowComponentTests()
    {
        cartProductsServiceFactoryMock.GetCartProductsService().Returns(cartProductsServiceMock);
        storeProductsServiceFactoryMock.GetStoreProductsService().Returns(storeProductsServiceMock);
        Services.AddSingleton(cartProductsServiceFactoryMock);
        Services.AddSingleton(storeProductsServiceFactoryMock);
        Services.AddSingleton(vm);
    }
    public static TheoryData<List<CartProductUIModel>> CartProductListData => new()
    {
        {
            new List<CartProductUIModel>()
            {
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 1", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 2", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000 },
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 3", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 }
            }
        },
        {
            new List<CartProductUIModel>()
            {
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 1", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 2", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000, IsCollected = true },
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 3", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 }
            }
        },
        {
            new List<CartProductUIModel>()
            {
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 1", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000, IsCollected = true },
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 2", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000, IsCollected = true },
                 new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "Product 3", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000, IsCollected = true }
            }
        },
    };

    [Theory]
    [MemberData(nameof(CartProductListData))]
    public void WhenCartHasItems_ShowsCorrectTotal(List<CartProductUIModel> products)
    {
        foreach (CartProductUIModel p in products)
        {
            vm.CartProducts.Add(p);
        }
        IRenderedComponent<CartSummaryRowComponent> cut = RenderComponent<CartSummaryRowComponent>();
        double expectedPrice = Math.Round(vm.CartProducts.Select(x => x.Amount * x.UnitPrice).Sum(), 2);
        Assert.Contains(expectedPrice.ToString("N2", CultureInfo.InvariantCulture), cut.Markup);
    }

    [Theory]
    [MemberData(nameof(CartProductListData))]
    public void WhenCartHasItems_AndClearCartProductsButtonIsPressed_CartIsCleared(List<CartProductUIModel> products)
    {
        foreach (CartProductUIModel p in products)
        {
            vm.CartProducts.Add(p);
        }
        IRenderedComponent<CartSummaryRowComponent> cut = RenderComponent<CartSummaryRowComponent>();
        IElement buttonElement = cut.Find("button[aria-label=\"Clear cart\"]");
        buttonElement.Click();
        Assert.Contains("Total: 0", cut.Markup);
    }

    [Fact]
    public void WhenStoreProductHasItems_AndClearStoreProductsButtonIsPressed_StoreProductsIsCleared()
    {
        IRenderedComponent<CartSummaryRowComponent> cut = RenderComponent<CartSummaryRowComponent>();
        IElement buttonElement = cut.Find("button[aria-label=\"Clear store products\"]");
        buttonElement.Click();
        Assert.Empty(vm.StoreProducts);
        storeProductsServiceMock.Received(1).ClearStoreProducts();
    }
}

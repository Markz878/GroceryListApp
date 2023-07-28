using AngleSharp.Dom;
using Bunit;
using Bunit.TestDoubles;
using GroceryListHelper.Client.Components;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Interfaces;
using GroceryListHelper.Shared.Models.StoreProducts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NSubstitute;

namespace GroceryListHelper.Tests.Client.bUnit;
public class CartComponentTests : TestContext
{
    private readonly MainViewModel mainVM = new();
    private readonly ModalViewModel modalVm = new();
    private readonly ICartProductsServiceFactory cartProductsServiceFactoryMock = Substitute.For<ICartProductsServiceFactory>();
    private readonly IStoreProductsServiceFactory storeProductsServiceFactoryMock = Substitute.For<IStoreProductsServiceFactory>();
    private readonly ICartProductsService cartProductsServiceMock = Substitute.For<ICartProductsService>();
    private readonly IStoreProductsService storeProductsServiceMock = Substitute.For<IStoreProductsService>();
    private readonly FakePersistentComponentState persist;
    public CartComponentTests()
    {
        persist = this.AddFakePersistentComponentState();
        Services.AddSingleton(cartProductsServiceFactoryMock);
        Services.AddSingleton(storeProductsServiceFactoryMock);
        cartProductsServiceFactoryMock.GetCartProductsService().Returns(cartProductsServiceMock);
        storeProductsServiceFactoryMock.GetStoreProductsService().Returns(storeProductsServiceMock);
        Services.AddSingleton(mainVM);
        Services.AddSingleton(modalVm);
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
    public void WhenCartHasItems_TheyAreShown(List<CartProductUIModel> products)
    {
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        foreach (CartProductUIModel p in products)
        {
            Assert.NotNull(cut.Find($"span:contains(\"{p.Name}\")"));
            Assert.NotNull(cut.Find($"span:contains(\"{p.Amount}\")"));
            Assert.NotNull(cut.Find($"span:contains(\"{p.UnitPrice:N2}\")"));
            Assert.NotNull(cut.Find($"span:contains(\"{p.Total:N2}\")"));
        }
        Assert.Equal(products.Count(x => x.IsCollected), cut.FindAll("input[checked]").Count);
        Assert.Equal(products.Count(x => x.IsCollected), cut.FindAll("div[role='row'].bg-gray-400").Count);
    }

    [Theory]
    [MemberData(nameof(CartProductListData))]
    public void WhenCartHasItems_AndItemsArePersisted_TheyAreShown(List<CartProductUIModel> products)
    {
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>() { new StoreProduct() { Name = "Product 1", UnitPrice = 2 } });
        persist.Persist(nameof(mainVM.CartProducts), products);
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        persist.TriggerOnPersisting();
        bool persisted = persist.TryTake(nameof(mainVM.CartProducts), out List<CartProductUIModel>? persistedProducts);
        Assert.True(persisted);
        Assert.NotNull(persistedProducts);
        Assert.Equal(products.Count, persistedProducts.Count);
        foreach (CartProductUIModel p in products)
        {
            Assert.NotNull(cut.Find($"span:contains(\"{p.Name}\")"));
            Assert.NotNull(cut.Find($"span:contains(\"{p.Amount}\")"));
            Assert.NotNull(cut.Find($"span:contains(\"{p.UnitPrice:N2}\")"));
            Assert.NotNull(cut.Find($"span:contains(\"{p.Total:N2}\")"));
        }
        Assert.Equal(products.Count(x => x.IsCollected), cut.FindAll("input[checked]").Count);
        Assert.Equal(products.Count(x => x.IsCollected), cut.FindAll("div[role='row'].bg-gray-400").Count);
    }

    [Fact]
    public async Task WhenProductsColumnIsPressed_ProductsAreSortedByNameAscending()
    {
        List<CartProductUIModel> products = new()
        {
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "DProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "EProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "AProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "CProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 4000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "BProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 5000 },
        };
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        IElement element = cut.Find("div[role='rowheader']>div>button[aria-label='Sort items']");
        await element.ClickAsync(new MouseEventArgs());
        IRefreshableElementCollection<IElement> rows = cut.FindAll("div[role=row]");
        IEnumerable<double> tops = rows.Select(x => double.Parse(x.Attributes["style"]?.Value.Replace("top: ", "").Replace("rem;", "") ?? ""));
        foreach (((string Name, int productIndex) First, (double top, int topIndex) Second) in products.Select((product, productIndex) => (product.Name, productIndex)).OrderBy(x => x.Name).Zip(tops.Select((top, topIndex) => (top, topIndex)).OrderBy(x => x.top)))
        {
            Assert.Equal(First.productIndex, Second.topIndex);
        }
    }

    [Fact]
    public async Task WhenProductsColumnIsPressedTwice_ProductsAreSortedByNameDescending()
    {
        List<CartProductUIModel> products = new()
        {
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "DProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "EProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "AProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "CProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 4000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "BProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 5000 },
        };
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        IElement element = cut.Find("div[role='rowheader']>div>button[aria-label='Sort items']");
        await element.ClickAsync(new MouseEventArgs());
        await Task.Delay(100);
        await element.ClickAsync(new MouseEventArgs());
        IRefreshableElementCollection<IElement> rows = cut.FindAll("div[role=row]");
        IEnumerable<double> tops = rows.Select(x => double.Parse(x.Attributes["style"]?.Value.Replace("top: ", "").Replace("rem;", "") ?? ""));
        foreach (((string Name, int productIndex) First, (double top, int topIndex) Second) in products.Select((product, productIndex) => (product.Name, productIndex)).OrderByDescending(x => x.Name).Zip(tops.Select((top, topIndex) => (top, topIndex)).OrderBy(x => x.top)))
        {
            Assert.Equal(First.productIndex, Second.topIndex);
        }
    }

    [Fact]
    public async Task WhenCartProductIsAdded_ServicesAreCalledAndItIsShownInTable()
    {
        cartProductsServiceMock.GetCartProducts().Returns(new List<CartProductUIModel>());
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        await cut.Find("input#newproduct-name-input").ChangeAsync(new ChangeEventArgs() { Value = "Product" });
        await cut.Find("input#newproduct-amount-input").ChangeAsync(new ChangeEventArgs() { Value = "2" });
        await cut.Find("input#newproduct-price-input").ChangeAsync(new ChangeEventArgs() { Value = "3.2" });
        await cut.Find("button#add-cartproduct-button").ClickAsync(new MouseEventArgs());
        await cartProductsServiceMock.Received().SaveCartProduct(Arg.Is<CartProduct>(x => x.Name == "Product" && x.Amount == 2 && x.UnitPrice == 3.2));
        await storeProductsServiceMock.Received().SaveStoreProduct(Arg.Is<StoreProduct>(x => x.Name == "Product" && x.UnitPrice == 3.2));
        Assert.Equal("Product", cut.Find("div[role='row']>span[aria-label='Product name']").InnerHtml);
    }

    [Fact]
    public async Task WhenInvalidCartProductIsAdded_ServicesAreNotCalledAndErrorIsShown()
    {
        cartProductsServiceMock.GetCartProducts().Returns(new List<CartProductUIModel>());
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        await cut.Find("input#newproduct-name-input").ChangeAsync(new ChangeEventArgs());
        await cut.Find("button#add-cartproduct-button").ClickAsync(new MouseEventArgs());
        await cartProductsServiceMock.DidNotReceive().SaveCartProduct(Arg.Any<CartProduct>());
        await storeProductsServiceMock.DidNotReceive().SaveStoreProduct(Arg.Any<StoreProduct>());
        Assert.Throws<ElementNotFoundException>(() => cut.Find("td#item-name-0"));
    }
}

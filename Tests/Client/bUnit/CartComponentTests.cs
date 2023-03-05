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
    private readonly ICartProductsService cartProductsServiceMock = Substitute.For<ICartProductsService>();
    private readonly IStoreProductsService storeProductsServiceMock = Substitute.For<IStoreProductsService>();
    private readonly FakePersistentComponentState persist;
    public CartComponentTests()
    {
        persist = this.AddFakePersistentComponentState();
        Services.AddSingleton(cartProductsServiceMock);
        Services.AddSingleton(storeProductsServiceMock);
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


    [Fact]
    public void WhenCartIsEmpty_NoProductsShown()
    {
        cartProductsServiceMock.GetCartProducts().Returns(new List<CartProductUIModel>());
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        cut.MarkupMatches($"""
            <table b-l8ub2h82qp>
              <thead b-l8ub2h82qp>
                <tr b-l8ub2h82qp>
                  <th b-l8ub2h82qp>Reorder</th>
                  <th b-l8ub2h82qp>Collected</th>
                  <th class="product-col" blazor:onclick="1" style="cursor: pointer" b-l8ub2h82qp>Product</th>
                  <th class="number-col" b-l8ub2h82qp>Amount</th>
                  <th class="number-col" b-l8ub2h82qp>Price</th>
                  <th b-l8ub2h82qp>Total</th>
                  <th b-l8ub2h82qp></th>
                  <th b-l8ub2h82qp></th>
                </tr>
              </thead>
              <tbody b-l8ub2h82qp>
                <tr b-l8ub2h82qp>
                  <td b-l8ub2h82qp></td>
                  <td b-l8ub2h82qp><button id="add-cartproduct-button" type="submit" class="btn btn-success" blazor:onclick="2" aria-label="Add product" b-l8ub2h82qp>Add</button></td>
                  <td b-l8ub2h82qp><input id="newproduct-name-input" type="text" list="products" class="form-control" aria-label="Product name input" autocomplete="off" blazor:onfocusout="3" value="" blazor:onchange="4" b-l8ub2h82qp blazor:elementReference="c0050e19-db88-40a7-bc57-62e11fd8fbef" /> <datalist id="products" b-l8ub2h82qp></datalist></td>
                  <td b-l8ub2h82qp><input id="newproduct-amount-input" type="number" step="1" min="1" class="form-control text-center" aria-label="Product amount input" value="1" blazor:onchange="5" b-l8ub2h82qp /></td>
                  <td b-l8ub2h82qp><input id="newproduct-price-input" type="number" step="0.01" min="0" class="form-control text-center" aria-label="Product unit price input" value="0" blazor:onchange="6" b-l8ub2h82qp /></td>
                  <td b-l8ub2h82qp></td>
                </tr>
              </tbody>
            </table>
            """);
    }

    [Theory]
    [MemberData(nameof(CartProductListData))]
    public void WhenCartHasItems_TheyAreShown(List<CartProductUIModel> products)
    {
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        foreach (var p in products)
        {
            Assert.NotNull(cut.Find($"td:contains(\"{p.Name}\")"));
            Assert.NotNull(cut.Find($"td:contains(\"{p.Amount}\")"));
            Assert.NotNull(cut.Find($"td:contains(\"{p.UnitPrice:N2}\")"));
            Assert.NotNull(cut.Find($"td:contains(\"{p.Total:N2}\")"));
        }
        Assert.Equal(products.Count(x => x.IsCollected), cut.FindAll("input[checked]").Count);
        Assert.Equal(products.Count(x => x.IsCollected), cut.FindAll("tr.checked-item").Count);
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
        foreach (var p in products)
        {
            Assert.NotNull(cut.Find($"td:contains(\"{p.Name}\")"));
            Assert.NotNull(cut.Find($"td:contains(\"{p.Amount}\")"));
            Assert.NotNull(cut.Find($"td:contains(\"{p.UnitPrice:N2}\")"));
            Assert.NotNull(cut.Find($"td:contains(\"{p.Total:N2}\")"));
        }
        Assert.Equal(products.Count(x=>x.IsCollected), cut.FindAll("input[checked]").Count);
        Assert.Equal(products.Count(x=>x.IsCollected), cut.FindAll("tr.checked-item").Count);
    }

    [Fact]
    public async Task WhenProductsColumnIsPressed_ProductsAreSortedByNameAscending()
    {
        List<CartProductUIModel> products = new()
        {
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "DProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "EProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "AProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "CProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "BProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000 },
        };
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        IElement element = cut.Find("th.product-col");
        await element.ClickAsync(new MouseEventArgs());
        IRefreshableElementCollection<IElement> names = cut.FindAll("td[aria-label='Product name']");
        foreach ((string name, IElement elem) in products.Select(x => x.Name).Order().Zip(names))
        {
            Assert.Equal(name, elem.InnerHtml);
        }
    }

    [Fact]
    public async Task WhenProductsColumnIsPressedTwice_ProductsAreSortedByNameDescending()
    {
        List<CartProductUIModel> products = new()
        {
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "DProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "EProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "AProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 1000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "CProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 3000 },
            new CartProductUIModel() { Amount = Random.Shared.Next(1,5), Name = "BProduct", UnitPrice = Random.Shared.NextDouble()*10, Order = 2000 },
        };
        cartProductsServiceMock.GetCartProducts().Returns(products);
        storeProductsServiceMock.GetStoreProducts().Returns(new List<StoreProduct>());
        IRenderedComponent<CartComponent> cut = RenderComponent<CartComponent>();
        IElement element = cut.Find("th.product-col");
        await element.ClickAsync(new MouseEventArgs());
        await Task.Delay(100);
        await element.ClickAsync(new MouseEventArgs());
        IRefreshableElementCollection<IElement> names = cut.FindAll("td[aria-label='Product name']");
        foreach ((string name, IElement elem) in products.Select(x => x.Name).OrderDescending().Zip(names))
        {
            Assert.Equal(name, elem.InnerHtml);
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
        Assert.Equal("Product", cut.Find("td#item-name-0").InnerHtml);
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

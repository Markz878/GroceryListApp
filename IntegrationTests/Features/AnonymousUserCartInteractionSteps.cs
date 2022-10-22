using GroceryListHelper.IntegrationTests.Models;
using GroceryListHelper.IntegrationTests.PageObjects;
using Microsoft.Playwright;
using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

namespace GroceryListHelper.IntegrationTests.Features;

[Binding]
public class AnonymousUserCartInteractionSteps
{
    private readonly IndexPageObject indexPage;
    private const string inputProductName = "Milk";
    private const int inputProductValidAmount = 1;
    private const int inputProductValidPrice = 2;

    public AnonymousUserCartInteractionSteps(IndexPageObject indexPage)
    {
        this.indexPage = indexPage;
    }

    [Given(@"a user in the index page")]
    public async Task GivenAUserInTheIndexPage()
    {
        await indexPage.NavigateAsync();
    }

    [When(@"a valid product's properties are written to the input fields")]
    public async Task WhenTheProductPropertiesAreWrittenToTheInputFields()
    {
        await indexPage.Page.FillAsync("#newproduct-name-input", inputProductName);
        await indexPage.Page.FillAsync("#newproduct-amount-input", inputProductValidAmount.ToString());
        await indexPage.Page.FillAsync("#newproduct-price-input", inputProductValidPrice.ToString());
    }

    [When(@"an invalid product's properties are written to the input fields")]
    public async Task WhenAnInvalidProductPropertiesAreWrittenToTheInputFields()
    {
        await indexPage.Page.FillAsync("#newproduct-name-input", inputProductName);
        await indexPage.Page.FillAsync("#newproduct-amount-input", "-5");
        await indexPage.Page.FillAsync("#newproduct-price-input", "-2");
    }

    [Then(@"an error message should be shown")]
    public async Task ThenAnErrorMessageShouldBeShown()
    {
        IElementHandle modalBody = await indexPage.Page.QuerySelectorAsync(".modal-body");
        string modalBodyText = await modalBody.InnerTextAsync();
        Assert.Equal("'Amount' must be greater than or equal to '0'. 'Unit Price' must be greater than or equal to '0'.", modalBodyText);
    }

    [When(@"the add button is clicked")]
    public async Task WhenTheAddButtonIsClicked()
    {
        await indexPage.Page.ClickAsync("#add-cartproduct-button");
    }

    [Then(@"the item should be added to the cart")]
    public async Task ThenTheItemShouldBeAddedToTheCart()
    {
        await Task.Delay(500);
        string cartProductsJson = await indexPage.Page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        CartProductCollectable[] models = JsonSerializer.Deserialize<CartProductCollectable[]>(cartProductsJson);
        Assert.True(models[0].Name == inputProductName);
        Assert.True(models[0].Amount == inputProductValidAmount);
        Assert.True(models[0].UnitPrice == inputProductValidPrice);
        Assert.True(models[0].IsCollected == false);
    }

    [Given(@"the following products in cart")]
    public async Task GivenTheFollowingProductsInCart(Table table)
    {
        foreach (TableRow item in table.Rows)
        {
            await indexPage.Page.FillAsync("#newproduct-name-input", item[0]);
            await indexPage.Page.FillAsync("#newproduct-amount-input", item[1]);
            await indexPage.Page.FillAsync("#newproduct-price-input", item[2]);
            await indexPage.Page.ClickAsync("#add-cartproduct-button");
        }
    }

    [When(@"the products are reordered using the Reorder-button")]
    public async Task WhenTheProductsAreReorderedUsingTheReorder_Button()
    {
        await indexPage.Page.ClickAsync("#reorder-button-0");
        await indexPage.Page.ClickAsync("#reorder-button-1");
    }

    [Then(@"the items should be in the order")]
    public async Task ThenTheFirstItemsShouldSwitchPlaces(Table table)
    {
        for (int i = 0; i < table.Rows.Count; i++)
        {
            IElementHandle item = await indexPage.Page.QuerySelectorAsync($"#item-name-{i}");
            string itemName = await item.InnerTextAsync();
            Assert.Equal(itemName, table.Rows[i][0]);
        }
    }

    [When(@"a user clicks the delete button on the item")]
    public async Task WhenAUserClicksTheDeleteButtonOnTheItem()
    {
        await indexPage.Page.ClickAsync("#delete-product-button-0");
    }

    [Then(@"the item should be removed from the cart")]
    public async Task ThenTheItemShouldBeRemovedFromTheCart()
    {
        Assert.False(await indexPage.Page.IsVisibleAsync("#item-name-0"));
    }

    [Then(@"the cart total should be (.*)")]
    public async Task ThenTheCartTotalShouldBe(int p0)
    {
        IElementHandle cartTotalElement = await indexPage.Page.QuerySelectorAsync($"#cart-total");
        string cartTotalString = await cartTotalElement.InnerTextAsync();
        Assert.Contains(p0.ToString(), cartTotalString);
    }

    [When(@"user starts to edit item")]
    public async Task WhenUserStartsToEditItem()
    {
        await indexPage.Page.ClickAsync("#edit-product-button-0");
    }

    [When(@"changes the item amount to (.*) and price to (.*)")]
    public async Task WhenChangesTheItemAmountToAndPriceTo(int amount, int price)
    {
        await indexPage.Page.FillAsync("#edit-item-amount-input-0", amount.ToString());
        await indexPage.Page.FillAsync("#edit-item-unitprice-input-0", price.ToString());
        await indexPage.Page.ClickAsync("#update-product-button-0");
    }

    [Then(@"the item amount should be (.*) and price (.*)")]
    public async Task ThenTheItemAmountShouldBeAndPrice(int amount, int price)
    {
        IElementHandle amountElement = await indexPage.Page.QuerySelectorAsync("#item-amount-0");
        string amountText = await amountElement.InnerTextAsync();

        IElementHandle priceElement = await indexPage.Page.QuerySelectorAsync("#item-unitprice-0");
        string priceText = await priceElement.InnerTextAsync();

        Assert.Equal(amount.ToString(), amountText);
        Assert.Equal(price.ToString(), priceText);
    }

    [When(@"the (.*) products are checked to have been collected")]
    public async Task WhenTheProductsAreCheckedToHaveBeenCollected(int p0)
    {
        for (int i = 0; i < p0; i++)
        {
            await indexPage.Page.CheckAsync($"#item-collected-checkbox-{i}");
        }
    }

    [Then(@"the all-collected check should say ""(.*)""")]
    public async Task ThenTheAll_CollectedCheckShouldSay(string p0)
    {
        IElementHandle collectedInfoElement = await indexPage.Page.QuerySelectorAsync("#cart-collected-info");
        string collectedInfoText = await collectedInfoElement.InnerTextAsync();
        Assert.Equal(p0, collectedInfoText);
    }
}

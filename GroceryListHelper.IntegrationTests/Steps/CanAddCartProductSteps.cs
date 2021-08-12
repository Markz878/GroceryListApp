using GroceryListHelper.Client.Models;
using GroceryListHelper.IntegrationTests.PageObjects;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace GroceryListHelper.IntegrationTests.Steps
{
    [Binding]
    public class CanAddCartProductSteps
    {
        private readonly IndexPageObject indexPage;

        public CanAddCartProductSteps(IndexPageObject indexPage)
        {
            this.indexPage = indexPage;
        }

        [Given(@"a user in the index page")]
        public async Task GivenAUserInTheIndexPage()
        {
            await indexPage.NavigateAsync();
        }

        [When(@"the product properties are written to the input fields")]
        public async Task WhenTheProductPropertiesAreWrittenToTheInputFields()
        {
            await indexPage.Page.FillAsync("#newproduct-name-input", "Milk");
            await indexPage.Page.FillAsync("#newproduct-amount-input", "1");
            await indexPage.Page.FillAsync("#newproduct-price-input", "2");
        }

        [When(@"the add button is clicked")]
        public async Task WhenTheAddButtonIsClicked()
        {
            await indexPage.Page.ClickAsync("#add-cartproduct-button");
        }

        [Then(@"the item should be added to the cart")]
        public async Task ThenTheItemShouldBeAddedToTheCart()
        {
            string cartProductsJson = await indexPage.Page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
            CartProductUIModel[] models = JsonSerializer.Deserialize<CartProductUIModel[]>(cartProductsJson);
            Assert.True(models[0].Id == 0);
            Assert.True(models[0].Amount == 1);
            Assert.True(models[0].UnitPrice == 2);
            Assert.True(models[0].Total == 2);
            Assert.True(models[0].IsCollected == false);
        }
    }

}

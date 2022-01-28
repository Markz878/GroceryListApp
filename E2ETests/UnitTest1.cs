using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace E2ETests
{
    public class WebServerTests : IClassFixture<WebServerFixture>
    {
        private readonly WebServerFixture fixture;

        public WebServerTests(WebServerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Page_title_equals_Welcome()
        {
            IPage page = await fixture.BrowserContext.NewPageAsync();
            await page.GotoAsync(fixture.BaseUrl);

            string actual = await page.ContentAsync();

            Assert.Contains("Grocery List Helper", actual);
        }
    }

    public static class Element
    {
        public static string ByName(string name)
        {
            return $"[pw-name='{name}']";
        }
    }
}

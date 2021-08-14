using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GroceryListHelper.IntegrationTests
{
    public class Tests : IClassFixture<WebApplicationFactory<Server.Startup>>
    {
        private readonly WebApplicationFactory<Server.Startup> _factory;

        public Tests(WebApplicationFactory<Server.Startup> factory)
        {
            _factory = factory;
        }


        [Theory]
        [InlineData("/")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html", response.Content.Headers.ContentType.ToString());
        }
    }
}

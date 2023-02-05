using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;
public class HealthTests : BaseTest
{
    public HealthTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }

    [Fact(Skip = "CI pipeline can't perform health check")]
    public async Task PingHealth_Success()
    {
        HttpResponseMessage result = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        string healthInfo = await result.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", healthInfo);
    }
}

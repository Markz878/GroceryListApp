﻿namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public class HealthTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : BaseTest(factory, testOutputHelper)
{
    [Fact]
    public async Task PingHealth_Success()
    {
        HttpResponseMessage result = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        string healthInfo = await result.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", healthInfo);
    }
}

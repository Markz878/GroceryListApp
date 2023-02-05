using GroceryListHelper.Shared.Models.Authentication;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;
using System.Text.Json;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

public sealed class GetUserInfoTests : BaseTest
{
    public GetUserInfoTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }

    [Fact]
    public async Task GetUserInfo_Success()
    {
        HttpResponseMessage result = await _client.GetAsync("api/account/user");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        string user = await result.Content.ReadAsStringAsync();
        UserInfo? userInfo = JsonSerializer.Deserialize<UserInfo>(user, _jsonOptions);
        Assert.NotNull(userInfo);
        Assert.True(userInfo.IsAuthenticated);
        Assert.True(userInfo.Claims?.Any());
    }
}

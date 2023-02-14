using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace GroceryListHelper.Tests.Client;

public class ClientAuthenticationStateProviderTests
{
    private readonly IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
    private readonly MockHttpMessageHandler _handlerMock = new();
    private readonly ClientAuthenticationStateProvider sut;

    public ClientAuthenticationStateProviderTests()
    {
        httpClientFactory.CreateClient("AnonymousClient").Returns(new HttpClient(_handlerMock) { BaseAddress = new Uri("https://localhost:5001") });
        sut = new(httpClientFactory, NullLogger<ClientAuthenticationStateProvider>.Instance);
    }

    [Fact]
    public async Task GetUserInfo_Success()
    {
        UserInfo returnValue = new()
        {
            IsAuthenticated = true,
            Claims = new List<ClaimValue>()
            {
                new ClaimValue("name", "Test User"),
                new ClaimValue("preferred_username", "test_user@email.com"),
                new ClaimValue("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString())
            }
        };
        MockedRequest request = _handlerMock
            .When(HttpMethod.Get, "https://localhost:5001/api/account/user")
            .Respond(HttpStatusCode.OK, JsonContent.Create(returnValue));

        AuthenticationState result1 = await sut.GetAuthenticationStateAsync();
        AuthenticationState result2 = await sut.GetAuthenticationStateAsync();

        // Assert that the expected behavior occurred
        Assert.True(result1.User.Identity?.IsAuthenticated);
        Assert.Equal(3, result1.User.Claims.Count());
        Assert.True(result2.User.Identity?.IsAuthenticated);
        Assert.Equal(3, result2.User.Claims.Count());

        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }

    [Fact]
    public async Task GetUserInfo_NotAuthenticated()
    {
        UserInfo returnValue = UserInfo.Anonymous;
        MockedRequest request = _handlerMock
            .When(HttpMethod.Get, "https://localhost:5001/api/account/user")
            .Respond(HttpStatusCode.OK, JsonContent.Create(returnValue));

        AuthenticationState result1 = await sut.GetAuthenticationStateAsync();
        AuthenticationState result2 = await sut.GetAuthenticationStateAsync();

        // Assert that the expected behavior occurred
        Assert.False(result1.User.Identity?.IsAuthenticated);
        Assert.Empty(result1.User.Claims);
        Assert.False(result2.User.Identity?.IsAuthenticated);
        Assert.Empty(result2.User.Claims);

        int matches = _handlerMock.GetMatchCount(request);
        Assert.Equal(1, matches);
    }
}

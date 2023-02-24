using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Tests.UnitTests;
public class ResponseModelTests
{
    [Fact]
    public void Success2Response()
    {
        Response<string, NotFound> response = "X";
        string result = response.Match(x => x, e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success2Error()
    {
        Response<string, NotFound> response = new(new NotFound("email"));
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception()));
    }

    [Fact]
    public void Success3Response()
    {
        Response<string, NotFound, Forbidden> response = "X";
        string result = response.Match(x => x, e => throw new Exception(), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success3Error1()
    {
        Response<string, NotFound, Forbidden> response = new(new NotFound("User"));
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public void Success3Error2()
    {
        Response<string, NotFound, Forbidden> response = new(new Forbidden());
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public async Task Success2ResponseAsync()
    {
        Response<string, NotFound> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success2ErrorAsync()
    {
        Response<string, NotFound> response = new(new NotFound("User"));
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception()));
    }

    [Fact]
    public async Task Success3ResponseAsync()
    {
        Response<string, NotFound, Forbidden> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success3Error1Async()
    {
        Response<string, NotFound, Forbidden> response = new(new NotFound("User"));
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public async Task Success3Error2Async()
    {
        Response<string, NotFound, Forbidden> response = new(new Forbidden());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception()));
    }
}

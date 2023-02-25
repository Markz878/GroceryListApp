using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Tests.UnitTests;
public class ResponseModelTests
{
    [Fact]
    public void Success2Response()
    {
        Response<string, NotFoundError> response = "X";
        string result = response.Match(x => x, e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success2Error()
    {
        Response<string, NotFoundError> response = new(new NotFoundError("email"));
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception()));
    }

    [Fact]
    public void Success3Response()
    {
        Response<string, NotFoundError, ForbiddenError> response = "X";
        string result = response.Match(x => x, e => throw new Exception(), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success3Error1()
    {
        Response<string, NotFoundError, ForbiddenError> response = new(new NotFoundError());
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public void Success3Error2()
    {
        Response<string, NotFoundError, ForbiddenError> response = new(new ForbiddenError());
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public async Task Success2ResponseAsync()
    {
        Response<string, NotFoundError> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success2ErrorAsync()
    {
        Response<string, NotFoundError> response = new(new NotFoundError());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception()));
    }

    [Fact]
    public async Task Success3ResponseAsync()
    {
        Response<string, NotFoundError, ForbiddenError> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success3Error1Async()
    {
        Response<string, NotFoundError, ForbiddenError> response = new(new NotFoundError());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public async Task Success3Error2Async()
    {
        Response<string, NotFoundError, ForbiddenError> response = new(new ForbiddenError());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception()));
    }
}

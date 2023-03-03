using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Tests.UnitTests;
public class ResponseModelTests
{
    [Fact]
    public void Success2Response()
    {
        Result<string, NotFoundError> response = "X";
        string result = response.Match(x => x, e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success2Error()
    {
        Result<string, NotFoundError> response = new(new NotFoundError("email"));
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception()));
    }

    [Fact]
    public void Success3Response()
    {
        Result<string, NotFoundError, ForbiddenError> response = "X";
        string result = response.Match(x => x, e => throw new Exception(), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success3Error1()
    {
        Result<string, NotFoundError, ForbiddenError> response = new(new NotFoundError());
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public void Success3Error2()
    {
        Result<string, NotFoundError, ForbiddenError> response = new(new ForbiddenError());
        Assert.Throws<Exception>(() => response.Match(x => x, e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public async Task Success2ResponseAsync()
    {
        Result<string, NotFoundError> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success2ErrorAsync()
    {
        Result<string, NotFoundError> response = new(new NotFoundError());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception()));
    }

    [Fact]
    public async Task Success3ResponseAsync()
    {
        Result<string, NotFoundError, ForbiddenError> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception());
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success3Error1Async()
    {
        Result<string, NotFoundError, ForbiddenError> response = new(new NotFoundError());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception()));
    }

    [Fact]
    public async Task Success3Error2Async()
    {
        Result<string, NotFoundError, ForbiddenError> response = new(new ForbiddenError());
        await Assert.ThrowsAsync<Exception>(() => response.MatchAsync(x => Task.FromResult(x), e => throw new Exception(), e => throw new Exception()));
    }
}

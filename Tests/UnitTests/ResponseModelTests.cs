using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Tests.UnitTests;
public class ResponseModelTests
{
    [Fact]
    public void Success2Response()
    {
        Response<string, NotFoundException> response = "X";
        string result = response.Match(x => x, e => throw e);
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success2Error()
    {
        Response<string, NotFoundException> response = new(new NotFoundException("email"));
        Assert.Throws<NotFoundException>(() => response.Match(x => x, e => throw e));
    }

    [Fact]
    public void Success3Response()
    {
        Response<string, NotFoundException, ForbiddenException> response = "X";
        string result = response.Match(x => x, e => throw e, e => throw e);
        Assert.Equal("X", result);
    }

    [Fact]
    public void Success3Error1()
    {
        Response<string, NotFoundException, ForbiddenException> response = new(new NotFoundException("User"));
        Assert.Throws<NotFoundException>(() => response.Match(x => x, e => throw e, e => throw e));
    }

    [Fact]
    public void Success3Error2()
    {
        Response<string, NotFoundException, ForbiddenException> response = new(new ForbiddenException());
        Assert.Throws<ForbiddenException>(() => response.Match(x => x, e => throw e, e => throw e));
    }

    [Fact]
    public async Task Success2ResponseAsync()
    {
        Response<string, NotFoundException> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw e);
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success2ErrorAsync()
    {
        Response<string, NotFoundException> response = new(new NotFoundException("User"));
        await Assert.ThrowsAsync<NotFoundException>(() => response.MatchAsync(x => Task.FromResult(x), e => throw e));
    }

    [Fact]
    public async Task Success3ResponseAsync()
    {
        Response<string, NotFoundException, ForbiddenException> response = "X";
        string result = await response.MatchAsync(x => Task.FromResult(x), e => throw e, e => throw e);
        Assert.Equal("X", result);
    }

    [Fact]
    public async Task Success3Error1Async()
    {
        Response<string, NotFoundException, ForbiddenException> response = new(new NotFoundException("User"));
        await Assert.ThrowsAsync<NotFoundException>(() => response.MatchAsync(x => Task.FromResult(x), e => throw e, e => throw e));
    }

    [Fact]
    public async Task Success3Error2Async()
    {
        Response<string, NotFoundException, ForbiddenException> response = new(new ForbiddenException());
        await Assert.ThrowsAsync<ForbiddenException>(() => response.MatchAsync(x => Task.FromResult(x), e => throw e, e => throw e));
    }
}

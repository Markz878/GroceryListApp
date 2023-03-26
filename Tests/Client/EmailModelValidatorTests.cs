using FluentValidation.Results;
using GroceryListHelper.Client.Models;

namespace GroceryListHelper.Tests.Client;

public class EmailModelValidatorTests
{
    private readonly EmailModelValidator validator = new();

    [Fact]
    public void WhenGivenValidEmail_ValidationPasses()
    {
        EmailModel emailModel = new() { Email = "test@gmail.com" };
        ValidationResult result = validator.Validate(emailModel);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@gmail")]
    [InlineData("@gmail.com")]
    [InlineData("afafaf@@afaf")]
    public void WhenGivenInvalidEmail_ValidationFails(string email)
    {
        EmailModel emailModel = new() { Email = email };
        ValidationResult result = validator.Validate(emailModel);
        Assert.False(result.IsValid);
    }
}

using Bogus;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.Controllers;
using GroceryListHelper.Shared.Models.CartProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace GroceryListHelper.UnitTests.ControllerTests;

public class CartProductsControllerTests
{
    private readonly CartProductsController cartProductsController;
    private readonly ICartProductRepository cartProductRepository;
    private readonly string userId = Guid.NewGuid().ToString();

    public CartProductsControllerTests()
    {
        cartProductRepository = Substitute.For<ICartProductRepository>();
        List<Claim> claims = new()
        {
            new Claim("id", userId.ToString())
        };
        ClaimsIdentity claimsIdentity = new(claims);
        cartProductsController = new CartProductsController(cartProductRepository)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            }
        };
    }

    [Fact]
    public async Task CartProductsController_Get_ReturnsIEnumerableOfCartProducts()
    {
        // Arrange
        List<CartProductCollectable> cartProducts = new Faker<CartProductCollectable>()
            .RuleFor(x => x.Id, x => Guid.NewGuid().ToString())
            .RuleFor(x => x.UnitPrice, x => x.Random.Double(0, 100))
            .RuleFor(x => x.Amount, x => x.Random.Int(0, 100))
            .RuleFor(x => x.Name, x => x.Random.Utf16String(3, 6))
            .RuleFor(x => x.IsCollected, x => x.Random.Bool())
            .Generate(10);
        cartProductRepository.GetCartProductsForUser(userId).Returns(Task.FromResult(cartProducts));
        // Act
        IActionResult actionResult = await cartProductsController.GetProducts();
        // Assert
        Assert.True(actionResult is OkObjectResult);
        OkObjectResult objectResult = actionResult as OkObjectResult;
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(cartProducts, objectResult.Value);
    }
}

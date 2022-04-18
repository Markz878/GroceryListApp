using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared.Exceptions;
using GroceryListHelper.Shared.Models.CartProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryListHelper.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CartProductsController : ControllerBase
{
    private readonly ICartProductRepository cartProductsRepository;

    public CartProductsController(ICartProductRepository cartProductsRepository)
    {
        this.cartProductsRepository = cartProductsRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CartProductCollectable>))]
    public async Task<IActionResult> GetProducts()
    {
        List<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(User.GetUserId());
        return Ok(results);
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartProductCollectable))]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        CartProductCollectable results = await cartProductsRepository.GetCartProductForUser(productId, User.GetUserId());
        return Ok(results);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddProduct(CartProduct product)
    {
        Guid id = await cartProductsRepository.AddCartProduct(product, User.GetUserId());
        return Created($"api/cartproducts/{id}", id);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAllProducts()
    {
        await cartProductsRepository.ClearProductsForUser(User.GetUserId());
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            await cartProductsRepository.DeleteProduct(id, User.GetUserId());
            return NoContent();
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(CartProductCollectable updatedProduct)
    {
        HttpContext context = HttpContext;
        try
        {
            await cartProductsRepository.UpdateProduct(User.GetUserId(), updatedProduct);
            return NoContent();
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}

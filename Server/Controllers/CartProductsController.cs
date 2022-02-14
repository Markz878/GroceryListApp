using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Models.CartProduct;
using Mapster;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartProductCollectable>))]
    public async Task<IActionResult> GetProducts()
    {
        IEnumerable<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(User.GetUserId());
        return Ok(results);
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartProductCollectable))]
    public async Task<IActionResult> GetProduct(string productId)
    {
        CartProductCollectable results = await cartProductsRepository.GetCartProductForUser(productId, User.GetUserId());
        return Ok(results);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CartProductCollectable))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddProduct(CartProduct product)
    {
        string id = await cartProductsRepository.AddCartProduct(product, User.GetUserId());
        CartProductCollectable createdProduct = product.Adapt<CartProductCollectable>();
        createdProduct.Id = id;
        return Created($"api/cartproducts/productId={id}", createdProduct);
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        bool success = await cartProductsRepository.DeleteProduct(id, User.GetUserId());
        return success ? NoContent() : NotFound();
    }

    //[HttpPatch("{id}")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> MarkAsCollected(string id)
    //{
    //    bool success = await cartProductsRepository.ToggleCollectedStatus(id, User.GetUserId());
    //    return success ? NoContent() : NotFound();
    //}

    //[HttpPatch("order/{id}")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> PatchOrder(string id)
    //{
    //    bool success = await cartProductsRepository.ToggleCollectedStatus(id, User.GetUserId());
    //    return success ? NoContent() : NotFound();
    //}

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(CartProductCollectable updatedProduct)
    {
        bool success = await cartProductsRepository.UpdateProduct(User.GetUserId(), updatedProduct);
        return success ? NoContent() : NotFound();
    }
}

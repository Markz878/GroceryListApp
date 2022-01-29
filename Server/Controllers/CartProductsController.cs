using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Models.CartProduct;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartProductsController : ControllerBase
{
    private readonly ICartProductRepository cartProductsRepository;

    public CartProductsController(ICartProductRepository cartProductsRepository)
    {
        this.cartProductsRepository = cartProductsRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        IEnumerable<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(User.GetUserId());
        return Ok(results);
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProduct(int productId)
    {
        CartProductCollectable results = await cartProductsRepository.GetCartProductForUser(productId, User.GetUserId());
        return Ok(results);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CartProductCollectable))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddProduct(CartProduct product)
    {
        int id = await cartProductsRepository.AddCartProduct(product, User.GetUserId());
        CartProductCollectable createdProduct = product.Adapt<CartProductCollectable>();
        createdProduct.Id = id;
        return Created($"api/cartproducts/productId={id}", createdProduct);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAllProducts()
    {
        await cartProductsRepository.RemoveItemsForUser(User.GetUserId());
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        bool success = await cartProductsRepository.DeleteItem(id, User.GetUserId());
        return success ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCollected(int id)
    {
        bool success = await cartProductsRepository.MarkAsCollected(id, User.GetUserId());
        return success ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, CartProduct updatedProduct)
    {
        bool success = await cartProductsRepository.UpdateProduct(id, User.GetUserId(), updatedProduct);
        return success ? NoContent() : NotFound();
    }
}

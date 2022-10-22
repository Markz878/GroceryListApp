using GroceryListHelper.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;

namespace GroceryListHelper.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StoreProductsController : ControllerBase
{
    private readonly IStoreProductRepository db;

    public StoreProductsController(IStoreProductRepository db)
    {
        this.db = db;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StoreProductDbModel[]))]
    public async Task<List<StoreProductServerModel>> GetProducts()
    {
        List<StoreProductServerModel> result = await db.GetStoreProductsForUser(User.GetUserId().GetValueOrDefault());
        return result;
    }

    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StoreProductDbModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        StoreProductServerModel result = await db.GetStoreProductForUser(id, User.GetUserId().GetValueOrDefault());
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    public async Task<IActionResult> Post(StoreProductModel product)
    {
        Guid id = await db.AddProduct(product, User.GetUserId().GetValueOrDefault());
        return Created($"api/storeproducts/{id}", id);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAllForUser()
    {
        await db.DeleteAll(User.GetUserId().GetValueOrDefault());
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await db.DeleteItem(id, User.GetUserId().GetValueOrDefault());
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrice(StoreProductServerModel storeProduct)
    {
        await db.UpdatePrice(storeProduct.Id, User.GetUserId().GetValueOrDefault(), storeProduct.UnitPrice);
        return NoContent();
    }
}

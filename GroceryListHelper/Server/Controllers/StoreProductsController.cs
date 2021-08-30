using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared.Models.StoreProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
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
        public IAsyncEnumerable<StoreProductResponseModel> GetProducts()
        {
            IAsyncEnumerable<StoreProductResponseModel> result = db.GetStoreProductsForUser(User.GetUserId());
            return result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> Post(StoreProductModel product)
        {
            int id = await db.AddProduct(product, User.GetUserId());
            return Ok(id);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAll()
        {
            await db.DeleteAll(User.GetUserId());
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await db.DeleteItem(id, User.GetUserId());
            return success ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePrice(int id, [FromQuery] double price)
        {
            if (price < 0)
            {
                return BadRequest("Price can't be negative.");
            }
            bool success = await db.UpdatePrice(id, User.GetUserId(), price);
            return success ? NoContent() : NotFound();
        }
    }
}

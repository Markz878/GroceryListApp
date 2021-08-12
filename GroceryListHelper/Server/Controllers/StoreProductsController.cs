using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
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
        public IAsyncEnumerable<StoreProductDbModel> GetProducts()
        {
            IAsyncEnumerable<StoreProductDbModel> result = db.GetStoreProductsForUser(User.GetUserId());
            return result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> Post(StoreProduct product)
        {
            int id = await db.AddProduct(product, User.GetUserId());
            return Ok(id);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAll()
        {
            await db.DeleteAll(User.GetUserId());
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await db.DeleteItem(id, User.GetUserId());
            return success ? Ok() : NotFound();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePrice(int id, [FromQuery] double price)
        {
            if (price < 0)
            {
                return BadRequest("Price can't be negative.");
            }
            bool success = await db.UpdatePrice(id, User.GetUserId(), price);
            return success ? Ok() : NotFound();
        }
    }
}

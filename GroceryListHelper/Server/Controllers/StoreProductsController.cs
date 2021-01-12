using GroceryListHelper.Server.Data;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Server.Models;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoreProductsController : ControllerBase
    {
        private readonly GroceryStoreDbContext db;

        public StoreProductsController(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await db.StoreProducts.Where(x => x.UserId == User.GetUserId()).AsNoTracking().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post(StoreProduct product)
        {
            StoreProductDbModel storeProduct = new() { Name = product.Name, UnitPrice = product.UnitPrice, UserId = User.GetUserId() };
            db.StoreProducts.Add(storeProduct);
            await db.SaveChangesAsync();
            return Ok(storeProduct.Id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            db.StoreProducts.RemoveRange(db.StoreProducts.Where(x => x.UserId == User.GetUserId()));
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = db.StoreProducts.Find(id);
            if (product != null)
            {
                if (product.UserId == User.GetUserId())
                {
                    db.StoreProducts.Remove(product);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound("No item with the given Id");
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePrice(int id, [FromQuery] double price)
        {
            if (price < 0)
            {
                return BadRequest("Price can't be negative.");
            }
            var product = db.StoreProducts.Find(id);
            if (product != null)
            {
                if (product.UserId == User.GetUserId())
                {
                    product.UnitPrice = price;
                    await db.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}

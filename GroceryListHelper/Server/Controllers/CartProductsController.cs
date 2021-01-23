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
    public class CartProductsController : ControllerBase
    {
        private readonly GroceryStoreDbContext db;

        public CartProductsController(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await db.CartProducts.Where(x => x.UserId == User.GetUserId()).AsNoTracking().ToListAsync();
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CartProduct product)
        {
            CartProductDbModel productDb = new CartProductDbModel() { Amount = product.Amount, Name = product.Name, UnitPrice = product.UnitPrice, UserId = User.GetUserId() };
            db.CartProducts.Add(productDb);
            await db.SaveChangesAsync();
            return Ok(productDb.Id);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            db.CartProducts.RemoveRange(db.CartProducts.Where(x => x.UserId == User.GetUserId()));
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = db.CartProducts.Find(id);
            if (product != null)
            {
                if (product.UserId == User.GetUserId())
                {
                    db.CartProducts.Remove(product);
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

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> MarkAsCollected(int id)
        {
            var product = db.CartProducts.Find(id);
            if (product != null)
            {
                if (product.UserId == User.GetUserId())
                {
                    product.IsCollected ^= true;
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, CartProduct updatedProduct)
        {
            var product = db.CartProducts.Find(id);
            if (product != null)
            {
                if (product.UserId == User.GetUserId())
                {
                    product.Name = updatedProduct.Name;
                    product.Amount = updatedProduct.Amount;
                    product.UnitPrice = updatedProduct.UnitPrice;
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

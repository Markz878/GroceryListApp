using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
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
        public async Task<IActionResult> Get()
        {
            IEnumerable<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(User.GetUserId());
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CartProduct product)
        {
            int id = await cartProductsRepository.AddCartProduct(product, User.GetUserId());
            return Ok(id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllProducts()
        {
            await cartProductsRepository.RemoveItemsForUser(User.GetUserId());
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            bool success = await cartProductsRepository.DeleteItem(id, User.GetUserId());
            return success ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> MarkAsCollected(int id)
        {
            bool success = await cartProductsRepository.MarkAsCollected(id, User.GetUserId());
            return success ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, CartProduct updatedProduct)
        {
            bool success = await cartProductsRepository.UpdateProduct(id, User.GetUserId(), updatedProduct);
            return success ? NoContent() : NotFound();
        }
    }
}

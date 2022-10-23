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
        List<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(User.GetUserId().GetValueOrDefault());
        return Ok(results);
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartProductCollectable))]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        CartProductCollectable results = await cartProductsRepository.GetCartProductForUser(productId, User.GetUserId().GetValueOrDefault());
        return Ok(results);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddProduct(CartProduct product)
    {
        Guid id = await cartProductsRepository.AddCartProduct(product, User.GetUserId().GetValueOrDefault());
        return Created($"api/cartproducts/{id}", id);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAllProducts()
    {
        await cartProductsRepository.ClearProductsForUser(User.GetUserId().GetValueOrDefault());
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await cartProductsRepository.DeleteProduct(id, User.GetUserId().GetValueOrDefault());
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(CartProductCollectable updatedProduct)
    {
        var ctx = HttpContext;
        await cartProductsRepository.UpdateProduct(User.GetUserId().GetValueOrDefault(), updatedProduct);
        return NoContent();
    }
}

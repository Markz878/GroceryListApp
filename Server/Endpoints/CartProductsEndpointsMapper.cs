namespace GroceryListHelper.Server.Endpoints;

public static class CartProductsEndpointsMapper
{
    public static void AddCartProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartproducts").RequireAuthorization();
        group.MapGet("", GetAll);
        group.MapPost("", AddProduct).AddEndpointFilterFactory(ValidatorFactory.Validator<CartProduct>);
        group.MapDelete("", DeleteAllProducts);
        group.MapPut("", UpdateProduct).AddEndpointFilterFactory(ValidatorFactory.Validator<CartProductCollectable>);
    }

    private static async Task<IResult> GetAll(ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        List<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(user.GetUserId().GetValueOrDefault());
        return TypedResults.Ok(results);
    }

    private static async Task<IResult> AddProduct(CartProduct product, ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        Guid id = await cartProductsRepository.AddCartProduct(product, user.GetUserId().GetValueOrDefault());
        return TypedResults.Created($"api/cartproducts", id);
    }

    private static async Task<IResult> DeleteAllProducts(ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.ClearProductsForUser(user.GetUserId().GetValueOrDefault());
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateProduct(CartProductCollectable updatedProduct, ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.UpdateProduct(user.GetUserId().GetValueOrDefault(), updatedProduct);
        return Results.NoContent();
    }
}

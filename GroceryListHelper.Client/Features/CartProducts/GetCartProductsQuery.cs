namespace GroceryListHelper.Client.Features.CartProducts;

internal sealed record GetCartProductsQuery : IRequest<List<CartProductCollectable>>
{
}

internal sealed class GetCartProductsQueryHandler(CartProductsServiceProvider cartProductsServiceProvider) : IRequestHandler<GetCartProductsQuery, List<CartProductCollectable>>
{
    public async Task<List<CartProductCollectable>> Handle(GetCartProductsQuery request, CancellationToken cancellationToken = default)
    {
        ICartProductsService service = await cartProductsServiceProvider.ResolveCartProductsService();
        return await service.GetCartProducts();
    }
}

namespace GroceryListHelper.Client.Features.CartProducts;

internal sealed record UpdateCartProductCommand : IRequest
{
    public required CartProductCollectable Product { get; init; }
}

internal sealed class UpdateCartProductCommandHandler(CartProductsServiceProvider cartProductsServiceProvider) : IRequestHandler<UpdateCartProductCommand>
{
    public async Task Handle(UpdateCartProductCommand request, CancellationToken cancellationToken = default)
    {
        ICartProductsService service = await cartProductsServiceProvider.ResolveCartProductsService();
        await service.UpdateCartProduct(request.Product);
    }
}

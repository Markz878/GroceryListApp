namespace GroceryListHelper.Client.Features.CartProducts;

internal sealed record DeleteCartProductCommand : IRequest
{
    public required string ProductName { get; init; }
}

internal sealed class DeleteCartProductCommandHandler(CartProductsServiceProvider cartProductsServiceProvider) : IRequestHandler<DeleteCartProductCommand>
{
    public async Task Handle(DeleteCartProductCommand request, CancellationToken cancellationToken = default)
    {
        ICartProductsService service = await cartProductsServiceProvider.ResolveCartProductsService();
        await service.DeleteCartProduct(request.ProductName);
    }
}

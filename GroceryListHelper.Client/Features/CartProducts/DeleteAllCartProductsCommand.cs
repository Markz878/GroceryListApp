namespace GroceryListHelper.Client.Features.CartProducts;

internal sealed record DeleteAllCartProductsCommand : IRequest
{
}

internal sealed class DeleteAllCartProductsCommandHandler(CartProductsServiceProvider cartProductsServiceProvider) : IRequestHandler<DeleteAllCartProductsCommand>
{
    public async Task Handle(DeleteAllCartProductsCommand request, CancellationToken cancellationToken = default)
    {
        ICartProductsService service = await cartProductsServiceProvider.ResolveCartProductsService();
        await service.DeleteAllCartProducts();
    }
}

namespace GroceryListHelper.Client.Features.CartProducts;

internal sealed record CreateCartProductCommand : IRequest
{
    public required CartProduct Product { get; init; }
}

internal sealed class CreateCartProductCommandHandler(CartProductsServiceProvider cartProductsServiceProvider) : IRequestHandler<CreateCartProductCommand>
{
    public async Task Handle(CreateCartProductCommand command, CancellationToken cancellationToken = default)
    {
        ICartProductsService service = await cartProductsServiceProvider.ResolveCartProductsService();
        await service.CreateCartProduct(command.Product);
    }
}
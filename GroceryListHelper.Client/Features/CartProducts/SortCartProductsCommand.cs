namespace GroceryListHelper.Client.Features.CartProducts;

internal sealed record SortCartProductsCommand : IRequest
{
    public required ListSortDirection SortDirection { get; init; }
}

internal sealed class SortCartProductsCommandHandler(CartProductsServiceProvider cartProductsServiceProvider) : IRequestHandler<SortCartProductsCommand>
{
    public async Task Handle(SortCartProductsCommand request, CancellationToken cancellationToken = default)
    {
        ICartProductsService service = await cartProductsServiceProvider.ResolveCartProductsService();
        await service.SortCartProducts(request.SortDirection);
    }
}

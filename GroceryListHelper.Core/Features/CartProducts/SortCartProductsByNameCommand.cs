using System.ComponentModel;

namespace GroceryListHelper.Core.Features.CartProducts;

public sealed record SortCartProductsByNameCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required ListSortDirection SortDirection { get; init; }
}

internal sealed class SortCartProductsByNameCommandHandler(IMediator mediator) : IRequestHandler<SortCartProductsByNameCommand>
{
    public async Task Handle(SortCartProductsByNameCommand request, CancellationToken cancellationToken = default)
    {
        List<CartProduct> cartProducts = await mediator.Send(new GetUserCartProductsQuery() { UserId = request.UserId });
        if (cartProducts.Count > 1)
        {
            SortProducts(cartProducts, request.SortDirection);
            await mediator.Send(new UpsertCartProductsCommand() { UserId = request.UserId, CartProducts = cartProducts }, cancellationToken);
        }
    }

    private static void SortProducts(List<CartProduct> cartProducts, ListSortDirection sortDirection)
    {
        int order = 1000;
        if (sortDirection is ListSortDirection.Ascending)
        {
            foreach (CartProduct item in cartProducts.OrderBy(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
        else
        {
            foreach (CartProduct item in cartProducts.OrderByDescending(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
    }
}


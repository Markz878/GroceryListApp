namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record GetCartGroupNameQuery : IRequest<Result<string, NotFoundError>>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
}

internal sealed class GetCartGroupNameQueryHandler(TableServiceClient db) : IRequestHandler<GetCartGroupNameQuery, Result<string, NotFoundError>>
{
    public async Task<Result<string, NotFoundError>> Handle(GetCartGroupNameQuery request, CancellationToken cancellationToken = default)
    {
        NullableResponse<CartGroupUserDbModel> groupName = await db.GetTableClient(CartGroupUserDbModel.GetTableName())
            .GetEntityIfExistsAsync<CartGroupUserDbModel>(
                request.GroupId.ToString(), request.UserEmail, GetNameColumn(), cancellationToken);
        CartGroupUserDbModel? value = groupName.Value;
        return value is null ? new NotFoundError() : value.Name;
    }

    static IEnumerable<string> GetNameColumn()
    {
        yield return "Name";
    }
}


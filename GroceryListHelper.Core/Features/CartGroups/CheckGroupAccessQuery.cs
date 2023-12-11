namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record CheckGroupAccessQuery : IRequest<bool>
{
    public required Guid GroupId { get; init; }
    public required string UserEmail { get; init; }
}

internal sealed class CheckGroupAccessQueryHandler(TableServiceClient db) : IRequestHandler<CheckGroupAccessQuery, bool>
{
    public async Task<bool> Handle(CheckGroupAccessQuery request, CancellationToken cancellationToken = default)
    {
        return await Common.CheckGroupAccess(db, request.GroupId, request.UserEmail, cancellationToken);
    }
}


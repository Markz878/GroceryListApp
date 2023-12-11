namespace GroceryListHelper.Core.Features.Users;

public sealed record CheckIfUserExistsQuery : IRequest<bool>
{
    public required string Email { get; init; }
}

internal sealed class CheckIfUserExistsQueryHandler(TableServiceClient db) : IRequestHandler<CheckIfUserExistsQuery, bool>
{
    public async Task<bool> Handle(CheckIfUserExistsQuery request, CancellationToken cancellationToken = default)
    {
        NullableResponse<UserDbModel> x = await db.GetTableClient(UserDbModel.GetTableName())
            .GetEntityIfExistsAsync<UserDbModel>(request.Email, request.Email, Array.Empty<string>(), cancellationToken);
        return x.HasValue;
    }
}


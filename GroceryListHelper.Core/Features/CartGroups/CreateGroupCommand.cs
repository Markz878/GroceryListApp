namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record CreateGroupCommand : IRequest<Result<Guid, NotFoundError>>
{
    public required string GroupName { get; init; }
    public required HashSet<string> UserEmails { get; init; }
}

internal sealed class CreateGroupCommandHandler(TableServiceClient db) : IRequestHandler<CreateGroupCommand, Result<Guid, NotFoundError>>
{
    public async Task<Result<Guid, NotFoundError>> Handle(CreateGroupCommand request, CancellationToken cancellationToken = default)
    {
        string? missingUserEmail = await CheckAllUsersFound(db, request.UserEmails, cancellationToken);
        if (string.IsNullOrEmpty(missingUserEmail) is false)
        {
            return new NotFoundError(missingUserEmail);
        }

        TableClient cartUserGroupTableClient = db.GetTableClient(CartUserGroupDbModel.GetTableName());
        TableClient cartGroupUserTableClient = db.GetTableClient(CartGroupUserDbModel.GetTableName());
        Guid groupId = await GetNewGroupId(cartGroupUserTableClient);
        foreach (string item in request.UserEmails)
        {
            await cartUserGroupTableClient.AddEntityAsync(new CartUserGroupDbModel()
            {
                MemberEmail = item,
                GroupId = groupId
            }, cancellationToken);
            await cartGroupUserTableClient.AddEntityAsync(new CartGroupUserDbModel()
            {
                GroupId = groupId,
                MemberEmail = item,
                Name = request.GroupName
            }, cancellationToken);
        }
        return groupId;

        static async Task<string?> CheckAllUsersFound(TableServiceClient db, IEnumerable<string> emails, CancellationToken cancellationToken)
        {
            TableClient userTableClient = db.GetTableClient(UserDbModel.GetTableName());
            foreach (string email in emails)
            {
                NullableResponse<UserDbModel> userFoundResponse = await userTableClient
                    .GetEntityIfExistsAsync<UserDbModel>(email, email, Array.Empty<string>(), cancellationToken);
                if (userFoundResponse.HasValue is false)
                {
                    return email;
                }
            }
            return null;
        }

        static async Task<Guid> GetNewGroupId(TableClient cartGroupUserTableClient)
        {
            Guid groupId;
            List<CartGroupUserDbModel> entities;
            do
            {
                groupId = Guid.NewGuid();
                entities = await cartGroupUserTableClient.GetTableEntitiesByPrimaryKey<CartGroupUserDbModel>(groupId.ToString());
            } while (entities.Count > 0);
            return groupId;
        }
    }
}


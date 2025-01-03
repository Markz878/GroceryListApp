namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record CreateGroupCommand : IRequest<Result<Guid, NotFoundError>>
{
    public required string GroupName { get; init; }
    public required HashSet<string> UserEmails { get; init; }
}

internal sealed class CreateGroupCommandHandler(CosmosClient db) : IRequestHandler<CreateGroupCommand, Result<Guid, NotFoundError>>
{
    public async Task<Result<Guid, NotFoundError>> Handle(CreateGroupCommand request, CancellationToken cancellationToken = default)
    {
        string? missingUserEmail = await CheckAllUsersFound(db, request.UserEmails, cancellationToken);
        if (string.IsNullOrEmpty(missingUserEmail) is false)
        {
            return new NotFoundError(missingUserEmail);
        }

        Container groupsContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartGroupsContainer);
        Guid groupId = await GetNewGroupId(groupsContainer);
        await groupsContainer.UpsertItemAsync(new CartGroupEntity()
        {
            Id = groupId,
            Name = request.GroupName,
            MemberEmails = request.UserEmails
        });

        return groupId;

        static async Task<string?> CheckAllUsersFound(CosmosClient db, IEnumerable<string> emails, CancellationToken cancellationToken)
        {
            Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.UsersContainer);
            foreach (string email in emails)
            {
                ResponseMessage userFoundResponse = await container.ReadItemStreamAsync(email, new PartitionKey(email));
                if (userFoundResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return email;
                }
            }
            return null;
        }

        static async Task<Guid> GetNewGroupId(Container groupsContainer)
        {
            Guid groupId = Guid.NewGuid();
            string groupString = groupId.ToString();
            ResponseMessage existsResponse = await groupsContainer.ReadItemStreamAsync(groupString, new PartitionKey(groupString));
            if(existsResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                return await GetNewGroupId(groupsContainer);
            }
            return groupId;
        }
    }
}


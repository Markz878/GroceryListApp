namespace GroceryListHelper.Core.Features.Users;

public sealed record AddUserCommand : IRequest
{
    public required string Email { get; init; }
    public required string Name { get; init; }
}

internal sealed class AddUserCommandHandler(CosmosClient db) : IRequestHandler<AddUserCommand>
{
    public async Task Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        Container container = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.UsersContainer);
        await container.UpsertItemAsync(new UserEntity() { Email = request.Email, Name = request.Name });
    }
}


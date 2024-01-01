namespace GroceryListHelper.Core.Features.Users;

public sealed record AddUserCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
}

internal sealed class AddUserCommandHandler(TableServiceClient client) : IRequestHandler<AddUserCommand, bool>
{
    public async Task<bool> Handle(AddUserCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            TableClient db = client.GetTableClient(UserDbModel.GetTableName());
            await db.AddEntityAsync(new UserDbModel() { Id = request.Id, Email = request.Email, Name = request.Name }, cancellationToken);
            return true;
        }
        catch (RequestFailedException ex) when (ex.Status is 409)
        {
            return false;
        }
    }
}


namespace GroceryListHelper.Core.Features.CartGroups;

public sealed record GetUserCartGroupsQuery : IRequest<List<CartGroup>>
{
    public required string UserEmail { get; init; }
}

internal sealed class GetCartGroupsQueryHandler(CosmosClient db) : IRequestHandler<GetUserCartGroupsQuery, List<CartGroup>>
{
    public async Task<List<CartGroup>> Handle(GetUserCartGroupsQuery request, CancellationToken cancellationToken = default)
    {
        Container groupsContainer = db.GetContainer(DataAccessConstants.Database, DataAccessConstants.CartGroupsContainer);
        string sql = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.memberEmails, @userEmail)";
        QueryDefinition query = new QueryDefinition(sql).WithParameter("@userEmail", request.UserEmail);
        List<CartGroup> groups = await groupsContainer.Query<CartGroupEntity, CartGroup>(x => new CartGroup()
        {
            Name = x.Name,
            Id = x.Id,
            OtherUsers = x.MemberEmails
        }, query);
        return groups;
    }
}




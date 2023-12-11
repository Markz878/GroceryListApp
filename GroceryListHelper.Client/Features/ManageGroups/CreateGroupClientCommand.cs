using GroceryListHelper.Shared.Models.CartGroups;
using MediatR;

namespace GroceryListHelper.Client.Features.ManageGroups;

public sealed record CreateGroupClientCommand : IRequest<Result<Guid, string>>
{
    public required CreateCartGroupRequest Request { get; init; }
}

internal sealed class CreateGroupClientCommandHandler(IHttpClientFactory httpClientFactory) : IRequestHandler<CreateGroupClientCommand, Result<Guid, string>>
{
    public async Task<Result<Guid, string>> Handle(CreateGroupClientCommand command, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClientFactory.CreateClient("Client").PostAsJsonAsync("api/cartgroups", command.Request, cancellationToken: cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        body = body.Trim('"');
        return response.IsSuccessStatusCode ? Guid.Parse(body) : body;
    }
}


namespace GroceryListHelper.Client.Features.ManageGroups;


public sealed record DeleteGroupClientCommand : IRequest<DeleteGroupClientCommandResponse>
{
    public required Guid GroupId { get; init; }
}

public sealed record DeleteGroupClientCommandResponse
{
    public string? Error { get; init; }
}

internal sealed class DeleteGroupClientCommandHandler(IHttpClientFactory httpClientFactory) : IRequestHandler<DeleteGroupClientCommand, DeleteGroupClientCommandResponse>
{
    public async Task<DeleteGroupClientCommandResponse> Handle(DeleteGroupClientCommand request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClientFactory.CreateClient("Client").DeleteAsync($"api/cartgroups/{request.GroupId}", cancellationToken);
        if (response.IsSuccessStatusCode is false)
        {
            return new DeleteGroupClientCommandResponse()
            {
                Error = await response.Content.ReadAsStringAsync(cancellationToken)
            };
        }
        return new();
    }
}


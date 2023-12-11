using GroceryListHelper.Shared.Models.CartGroups;
using System.Net.Http;
using System;

namespace GroceryListHelper.Client.Features.ManageGroups;


public sealed record UpdateGroupNameClientCommand : IRequest<UpdateGroupNameClientCommandResponse>
{
    public required Guid GroupId { get; init; }
    public required string GroupName { get; init; }
}

public sealed record UpdateGroupNameClientCommandResponse
{
    public string? Error { get; init; }
}

internal sealed class UpdateGroupNameClientCommandHandler(IHttpClientFactory httpClientFactory) : IRequestHandler<UpdateGroupNameClientCommand, UpdateGroupNameClientCommandResponse>
{
    public async Task<UpdateGroupNameClientCommandResponse> Handle(UpdateGroupNameClientCommand request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClientFactory.CreateClient("Client").PutAsJsonAsync($"api/cartgroups/{request.GroupId}", new UpdateCartGroupNameRequest() { Name = request.GroupName }, cancellationToken: cancellationToken);
        if (response.IsSuccessStatusCode is false)
        {
            return new UpdateGroupNameClientCommandResponse()
            {
                Error = await response.Content.ReadAsStringAsync(cancellationToken)
            };
        }
        return new();
    }
}


using System.Text.Json.Serialization;

namespace GroceryListHelper.Core.DataAccess.Models;

internal sealed record StoreProductEntity
{
    [JsonPropertyName("id")]
    public required string Name { get; init; }
    public required Guid UserId { get; init; }
    public required double UnitPrice { get; init; }
}

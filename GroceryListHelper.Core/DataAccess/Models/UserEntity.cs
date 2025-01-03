using System.Text.Json.Serialization;

namespace GroceryListHelper.Core.DataAccess.Models;

internal sealed record UserEntity
{
    [JsonPropertyName("id")]
    public required string Email { get; set; }
    public string? Name { get; set; }
}

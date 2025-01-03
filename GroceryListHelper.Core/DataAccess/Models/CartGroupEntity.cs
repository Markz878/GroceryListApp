using System.Text.Json.Serialization;

namespace GroceryListHelper.Core.DataAccess.Models;

internal sealed record CartGroupEntity
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public HashSet<string> MemberEmails { get; set; } = [];
}

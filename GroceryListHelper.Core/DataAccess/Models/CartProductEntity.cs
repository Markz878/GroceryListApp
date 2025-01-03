using System.Text.Json.Serialization;

namespace GroceryListHelper.Core.DataAccess.Models;

internal sealed record CartProductEntity
{
    [JsonPropertyName("id")]
    public required string Name { get; set; }
    public Guid UserId { get; set; }
    public double UnitPrice { get; set; }
    public double Amount { get; set; } = 1;
    public double Order { get; set; }
    public bool IsCollected { get; set; }
}

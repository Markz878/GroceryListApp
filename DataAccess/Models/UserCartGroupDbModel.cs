namespace GroceryListHelper.DataAccess.Models;

public class UserCartGroupDbModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Guid HostId { get; set; }
    public string HostEmail { get; set; }
    public string JoinerEmail { get; set; }
}

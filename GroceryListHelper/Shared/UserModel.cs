using System.ComponentModel.DataAnnotations;

namespace GroceryListHelper.Shared
{
    public class UserModel
    {
        [Required]
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}

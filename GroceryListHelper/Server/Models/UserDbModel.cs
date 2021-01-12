namespace GroceryListHelper.Server.Models
{
    public class UserDbModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string RefreshToken { get; set; }
    }
}

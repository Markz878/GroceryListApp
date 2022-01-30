using GroceryListHelper.DataAccess.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace GroceryListHelper.DataAccess.HelperMethods;

public static class PasswordHelper
{
    public static bool CheckPassword(UserDbModel user, string password)
    {
        return user.PasswordHash == HashPassword(password, user.Salt);
    }

    internal static string HashPassword(string password, byte[] salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 10000, 512 / 8));
    }

    internal static byte[] GenerateSalt()
    {
        byte[] salt = new byte[512 / 8];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
}

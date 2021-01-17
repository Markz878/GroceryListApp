using GroceryListHelper.Server.Data;
using GroceryListHelper.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.HelperMethods
{
    public class JWTAuthenticationManager
    {
        private readonly GroceryStoreDbContext db;
        private readonly IConfiguration configuration;
        private const string AccessTokenKey = "AccessTokenKey";
        private const string RefreshTokenKey = "RefreshTokenKey";

        public JWTAuthenticationManager(IConfiguration configuration, GroceryStoreDbContext db)
        {
            this.db = db;
            this.configuration = configuration;
        }

        /// <summary>
        /// Registers a new user. Checks first if there is already a user with the given email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Returns an error message if email is already in use, otherwise null.</returns>
        internal async Task<string> Register(string email, string password)
        {
            if (db.Users.Any(u => u.Email.Equals(email)))
            {
                return "Email is already in use.";
            }
            else
            {
                byte[] salt = PasswordHelper.GenerateSalt();
                UserDbModel user = new UserDbModel() { Email = email, Salt = salt, PasswordHash = PasswordHelper.HashPassword(password, salt) };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return null;
            }
        }

        internal async Task<string> ChangePassword(string email, string currentPassword, string newPassword)
        {
            UserDbModel user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return "User not found";
            }
            else
            {
                if (user.PasswordHash.Equals(PasswordHelper.HashPassword(currentPassword, user.Salt)))
                {
                    user.Salt = PasswordHelper.GenerateSalt();
                    user.PasswordHash = PasswordHelper.HashPassword(newPassword, user.Salt);
                    await db.SaveChangesAsync();
                    return null;
                }
                else
                {
                    return "Invalid password";
                }
            }
        }

        /// <summary>
        /// Authenticates the user and returns an access token if the email and password are valid.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Authentication JWT or null if failed.</returns>
        internal async Task<string> GetUserAccessToken(string email, string password)
        {
            UserDbModel user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return null;
            }
            else
            {
                if (user.PasswordHash.Equals(PasswordHelper.HashPassword(password, user.Salt)))
                {
                    return GenerateAccessToken(user.Id.ToString(), email);
                }
            }
            return null;
        }

        /// <summary>
        /// Generates a new access token from the old token by reading the listed claims.
        /// Does not validate expiration, as it is assumed that the old token has expired.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        internal string RefreshAccessToken(string accessToken, string refreshToken)
        {
            if (ValidateToken(AccessTokenKey, accessToken, false, out ClaimsPrincipal atClaimsPrincipal) && ValidateToken(RefreshTokenKey, refreshToken, false, out ClaimsPrincipal rtClaimsPrincipal) && atClaimsPrincipal.FindFirstValue(ClaimTypes.Email).Equals(rtClaimsPrincipal.FindFirstValue(ClaimTypes.Email)))
            {
                return GenerateAccessToken(rtClaimsPrincipal.FindFirstValue("Id"), rtClaimsPrincipal.FindFirstValue(ClaimTypes.Email));
            }
            return null;
        }


        private string GenerateAccessToken(string id, string email)
        {
            return GenerateToken(AccessTokenKey, TimeSpan.FromMinutes(double.Parse(configuration["AccessTokenLifeTimeMinutes"])), id, email);
        }

        internal async Task<string> DeleteUser(string email, string password)
        {
            UserDbModel user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return "User not found";
            }
            else
            {
                if (user.PasswordHash.Equals(PasswordHelper.HashPassword(password, user.Salt)))
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return null;
                }
            }
            return "Invalid username or password";
        }

        private string GenerateRefreshToken(string id, string email)
        {
            return GenerateToken(RefreshTokenKey, TimeSpan.FromMinutes(double.Parse(configuration["AccessTokenLifeTimeMinutes"])), id, email);
        }

        private string GenerateToken(string key, TimeSpan duration, string id, string email)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(configuration[key]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["ServerURL"],
                Audience = configuration["ClientURL"],
                Subject = new ClaimsIdentity(new Claim[] { new Claim("Id", id), new Claim(ClaimTypes.Email, email) }),
                Expires = DateTime.UtcNow + duration,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// Returns a refresh token with 1 day lifetime from reading the claims in the given access token.
        /// Access token is not validated for expiration.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>Refresh JWT or null if token signature was invalid.</returns>
        internal async Task<string> GetUserRefreshToken(string accessToken)
        {
            if (ValidateToken(AccessTokenKey, accessToken, false, out ClaimsPrincipal claimsPrincipal))
            {
                string email = claimsPrincipal?.FindFirstValue(ClaimTypes.Email);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
                if (user != null)
                {
                    user.RefreshToken = GenerateRefreshToken(claimsPrincipal.FindFirstValue("Id"), claimsPrincipal.FindFirstValue(ClaimTypes.Email));
                    await db.SaveChangesAsync();
                    return user.RefreshToken;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a refresh JWT with 1 day lifetime based on the previous refresh token claims.
        /// Checks that the previous refresh token has not expired and is otherwise valid, and copies the claims from it.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Refreshed refresh JWT or null if previous token was invalid.</returns>
        internal async Task<string> RenewRefreshToken(string refreshToken)
        {
            if (ValidateToken(RefreshTokenKey, refreshToken, true, out ClaimsPrincipal claimsPrincipal))
            {
                string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email) && u.RefreshToken.Equals(refreshToken));
                if (user != null)
                {
                    user.RefreshToken = GenerateRefreshToken(claimsPrincipal.FindFirstValue("Id"), claimsPrincipal.FindFirstValue(ClaimTypes.Email));
                    await db.SaveChangesAsync();
                    return user.RefreshToken;
                }
            }
            return null;
        }

        private bool ValidateToken(string key, string token, bool validateLifetime, out ClaimsPrincipal claimsPrincipal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[key])),
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime
            };
            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken jwt);
                return true;
            }
            catch
            {
                if (tokenHandler.CanReadToken(token))
                {
                    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(tokenHandler.ReadJwtToken(token).Claims));
                }
                else
                {
                    claimsPrincipal = null;
                }
                return false;
            }
        }
    }
}

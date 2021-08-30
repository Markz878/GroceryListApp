using GroceryListHelper.Shared.Models.Authentication;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.HelperMethods
{
    public interface IJWTAuthenticationManager
    {
        Task<(AuthenticationResponseModel response, string refreshToken)> Login(string email, string password);
        Task<(AuthenticationResponseModel response, string newRefreshToken)> RefreshTokens(string oldRefreshToken);
        Task<(AuthenticationResponseModel response, string refreshToken)> Register(string email, string password);
    }
}
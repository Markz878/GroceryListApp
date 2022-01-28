using System.Threading.Tasks;

namespace GroceryListHelper.Client.Authentication;

public interface IAccessTokenProvider
{
    ValueTask RemoveToken();
    ValueTask<string> RequestAccessToken();
    ValueTask SaveToken(string accessToken);
    ValueTask<string> TryToRefreshToken();
}

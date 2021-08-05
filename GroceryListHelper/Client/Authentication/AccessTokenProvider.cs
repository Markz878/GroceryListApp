using GroceryListHelper.Shared;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Authentication
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly HttpClient client;
        private readonly IJSRuntime jsRuntime;
        private const string accessTokenKey = "AccessToken";
        private string accessToken; // Null => try to get, String.Empty => Skip trying

        public AccessTokenProvider(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
        {
            client = httpClientFactory.CreateClient("AnonymousClient");
            this.jsRuntime = jsRuntime;
        }

        public async ValueTask<string> RequestAccessToken()
        {
            if (accessToken != null)
            {
                return accessToken;
            }
            string accuiredToken = await jsRuntime.InvokeAsync<string>("localStorage.getItem", accessTokenKey);
            if (string.IsNullOrEmpty(accuiredToken))
            {
                accuiredToken = await TryToRefreshToken();
            }
            accessToken = accuiredToken;
            if (accessToken == null)
            {
                accessToken = string.Empty;
            }
            return accuiredToken?.Trim('"');
        }

        public async ValueTask<string> TryToRefreshToken()
        {
            HttpResponseMessage response = await client.GetAsync("api/authentication/refresh");
            if (response.IsSuccessStatusCode)
            {
                AuthenticationResponseModel loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                await SaveToken(loginResponse.AccessToken);
                return loginResponse.AccessToken;
            }
            else
            {
                return null;
            }
        }

        public ValueTask SaveToken(string accessToken)
        {
            this.accessToken = accessToken;
            return jsRuntime.InvokeVoidAsync("localStorage.setItem", accessTokenKey, accessToken.Trim('"'));
        }

        public ValueTask RemoveToken()
        {
            accessToken = string.Empty;
            return jsRuntime.InvokeVoidAsync("localStorage.removeItem", accessTokenKey);
        }
    }
}

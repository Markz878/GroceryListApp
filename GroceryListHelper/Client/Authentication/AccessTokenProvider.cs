using GroceryListHelper.Shared;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Authentication
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly HttpClient client;
        private string accessToken;

        public AccessTokenProvider(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("AnonymousClient");
        }

        public async ValueTask<string> RequestAccessToken()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await TryToRefreshToken();
            }
            return accessToken;
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
            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveToken()
        {
            accessToken = string.Empty;
            return ValueTask.CompletedTask;
        }
    }
}
